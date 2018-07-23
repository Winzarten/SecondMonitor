namespace SecondMonitor.PCarsConnector
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.InteropServices;
    using System.Threading;

    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;
    using SecondMonitor.PluginManager.GameConnector;

    public class PCarsConnector : IGameConnector
    {

        public class NameNotFilledException : Exception
        {
            public NameNotFilledException(string msg) : base(msg)
            {
            }
        }

        private static readonly string[] PCarsExecutables =  { "pCARS64", "pCARS2" };
        private static readonly string SharedMemoryName = "$pcars$";
        private readonly Queue<SimulatorDataSet> _queue = new Queue<SimulatorDataSet>();
        private readonly PCarsConvertor _pCarsConvertor;

        private int _sharedMemorySize;
        private byte[] _sharedMemoryReadBuffer;
        private bool _isSharedMemoryInitialized;
        private GCHandle _handle;
        private MemoryMappedFile _sharedMemory;
        private Process _process;
        private Thread _daemonThread;
        private bool _disconnect;

        private DateTime _lastTick = DateTime.Now;

        public event EventHandler<DataEventArgs> DataLoaded;

        public event EventHandler<EventArgs> ConnectedEvent;

        public event EventHandler<EventArgs> Disconnected;

        public event EventHandler<DataEventArgs> SessionStarted;

        public event EventHandler<MessageArgs> DisplayMessage;

        private double _nextSpeedComputation;

        internal double NextSpeedComputation
        {
            get  => _nextSpeedComputation;
            set => _nextSpeedComputation = value;
        }

        private SimulatorDataSet _previousSet = new SimulatorDataSet("PCARS");

        public PCarsConnector()
        {
            TickTime = 10;
            ResetConnector();
            _pCarsConvertor = new PCarsConvertor(this);
        }

        private void ResetConnector()
        {
            SessionTime = TimeSpan.Zero;
            _process = null;
            _disconnect = false;
        }

        public bool IsConnected => _sharedMemory != null && _isSharedMemoryInitialized;

        public int TickTime
        {
            get;
            set;
        }

        public bool IsPCarsRunning()
        {
            if (_process != null)
            {
                if (!_process.HasExited)
                {
                    return true;
                }

                _process = null;
                return false;
            }

            foreach (var processName in PCarsExecutables)
            {
                var processes = Process.GetProcessesByName(processName);
                if (processes.Length > 0)
                {
                    _process = processes[0];
                    return true;
                }
            }

            return false;
        }

        public void ASyncConnect()
        {
            Thread asyncConnectThread = new Thread(ASynConnector);
            asyncConnectThread.IsBackground = true;
            asyncConnectThread.Start();
        }

        public bool TryConnect()
        {
            return Connect();
        }

        private bool Connect()
        {
            if (!IsPCarsRunning())
            {
                return false;
            }

            try
            {
                _sharedMemory = MemoryMappedFile.OpenExisting(SharedMemoryName);
                _sharedMemorySize = Marshal.SizeOf(typeof(PCarsApiStruct));
                _sharedMemoryReadBuffer = new byte[_sharedMemorySize];
                _isSharedMemoryInitialized = true;
                RaiseConnectedEvent();
                StartDaemon();
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        internal TimeSpan SessionTime { get; set; } = TimeSpan.Zero;

        internal uint LastSessionState { get; set; } = 0;

        private void ASynConnector()
        {
            while (!TryConnect())
            {
                Thread.Sleep(10);
            }
        }

        internal Dictionary<string, DriverInfo> PreviousTickInfo { get; set; } = new Dictionary<string, DriverInfo>();

        private void StartDaemon()
        {
            if (_daemonThread != null && _daemonThread.IsAlive)
            {
                throw new InvalidOperationException("Daemon is already running");
            }

            lock (_queue)
            {
                _queue.Clear();
            }

            ResetConnector();
            _daemonThread = new Thread(DaemonMethod);
            _daemonThread.IsBackground = true;
            _daemonThread.Start();

            Thread queueProcessorThread = new Thread(QueueProcessor) {IsBackground = true};
            queueProcessorThread.Start();
        }

        private void DaemonMethod()
        {
            while (!_disconnect)
            {
                Thread.Sleep(TickTime);
                PCarsApiStruct data = Load();
                try
                {
                    // This a state that sometimes occurs when saving pit preset during race
                    // This state is ignored, otherwise it would trigger a session reset
                    if (data.MSessionState == 0 && (data.MGameState == 2 || data.MGameState == 3))
                    {
                        continue;
                    }

                    DateTime tickTime = DateTime.Now;
                    TimeSpan lastTickDuration = tickTime.Subtract(_lastTick);
                    SimulatorDataSet simData= _pCarsConvertor.FromPcarsData(data, lastTickDuration);

                    if (ShouldResetSession(data))
                    {
                        _pCarsConvertor.Reset();
                        PreviousTickInfo.Clear();
                        _nextSpeedComputation = 0;
                        RaiseSessionStartedEvent(simData);
                    }

                    LastSessionState = data.MSessionState;
                    lock (_queue)
                    {
                        _queue.Enqueue(simData);
                    }

                    if (!IsPCarsRunning())
                    {
                        _disconnect = true;
                    }

                    _lastTick = tickTime;
                    _previousSet = simData;
                }
                catch (NameNotFilledException)
                {
                    // Ignore, names are sometimes not set in the shared memory
                }
            }

            _sharedMemory = null;
            RaiseDisconnectedEvent();
        }


        private bool ShouldResetSession(PCarsApiStruct data)
        {
            if (LastSessionState != data.MSessionState)
            {
                return true;
            }

            return _previousSet.SessionInfo.SessionTimeRemaining - data.MEventTimeRemaining < -5;
        }

        private void QueueProcessor()
        {
            while (_disconnect == false)
            {
                while (_queue.Count != 0)
                {
                    SimulatorDataSet set;
                    lock (_queue)
                    {
                        set = _queue.Dequeue();
                    }

                    RaiseDataLoadedEvent(set);
                }

                Thread.Sleep(TickTime);
            }

            _queue.Clear();
        }

        private PCarsApiStruct Load()
        {
            lock (_sharedMemory)
            {
                PCarsApiStruct pCarsApiStruct;
                if (!IsConnected)
                {
                    throw new InvalidOperationException("Not connected");
                }

                using (var sharedMemoryStreamView = _sharedMemory.CreateViewStream())
                {
                    BinaryReader sharedMemoryStream = new BinaryReader(sharedMemoryStreamView);
                    _sharedMemoryReadBuffer = sharedMemoryStream.ReadBytes(_sharedMemorySize);
                    _handle = GCHandle.Alloc(_sharedMemoryReadBuffer, GCHandleType.Pinned);
                    pCarsApiStruct = (PCarsApiStruct)Marshal.PtrToStructure(_handle.AddrOfPinnedObject(), typeof(PCarsApiStruct));
                    _handle.Free();
                }

                return pCarsApiStruct;
            }
        }


        private void RaiseDataLoadedEvent(SimulatorDataSet simData)
        {

            DataEventArgs args = new DataEventArgs(simData);
            DataLoaded?.Invoke(this, args);
        }

        private void RaiseSessionStartedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            EventHandler<DataEventArgs> handler = SessionStarted;
            _lastTick = DateTime.Now;
            SessionTime = new TimeSpan(0, 0, 1);
            handler?.Invoke(this, args);
        }

        private void RaiseConnectedEvent()
        {
            EventArgs args = new EventArgs();
            ConnectedEvent?.Invoke(this, args);
        }

        private void RaiseDisconnectedEvent()
        {
            EventArgs args = new EventArgs();
            Disconnected?.Invoke(this, args);
        }

        protected virtual void OnDisplayMessage(MessageArgs e)
        {
            DisplayMessage?.Invoke(this, e);
        }
    }
}
