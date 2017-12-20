using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using SecondMonitor.DataModel;
using SecondMonitor.PluginManager.GameConnector;
using System.Collections.Generic;
using SecondMonitor.DataModel.Drivers;

namespace SecondMonitor.PCarsConnector
{

    public class PCarsConnector : IGameConnector
    {

        public class NameNotFilledException : Exception
        {       
            public NameNotFilledException(string msg) : base(msg)
            {
            }
        }

        private MemoryMappedFile _sharedMemory;
        private static int _sharedmemorysize;
        private static byte[] _sharedMemoryReadBuffer;
        private static bool _isSharedMemoryInitialised = false;
        private static GCHandle _handle;
        private Process _process;
        private Thread _daemonThread;
        private bool _disconnect;
        private readonly Queue<SimulatorDataSet> _queue = new Queue<SimulatorDataSet>();

        private DateTime _lastTick = DateTime.Now;
        private TimeSpan _lastSessionTimeLeft;
        private TimeSpan _sessionTime = TimeSpan.Zero;

        private static readonly string[] PCarsExecutables = new string[] {"pCARS64", "pCARS2" };
        private static readonly string SharedMemoryName = "$pcars$";

        public event EventHandler<DataEventArgs> DataLoaded;
        public event EventHandler<EventArgs> ConnectedEvent;
        public event EventHandler<EventArgs> Disconnected;
        public event EventHandler<DataEventArgs> SessionStarted;
        
        private double _nextSpeedComputation;

        internal double NextSpeedComputation
        {
            get  => _nextSpeedComputation;
            set => _nextSpeedComputation = value;
        }
       
        private uint _lastSessionState = 0;
        private SimulatorDataSet _previousSet = new SimulatorDataSet("PCARS");
        private Dictionary<string, DriverInfo> _previousTickInfo = new Dictionary<string, DriverInfo>();
        private readonly PCarsConvertor _pCarsConvertor;


        public PCarsConnector()
        {
            TickTime = 10;
            ResetConnector();
            _pCarsConvertor = new PCarsConvertor(this);
        }

        private void ResetConnector()
        {
            _sessionTime = new TimeSpan(0);
            _process = null;
            _disconnect = false;
        }

        public bool IsConnected
        {
            get { return (_sharedMemory != null && _isSharedMemoryInitialised); }
        }

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

        public void AsynConnect()
        {
            Thread asyncConnectThread = new Thread(AsynConnector);
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
                _sharedmemorysize = Marshal.SizeOf(typeof(PCarsApiStruct));
                _sharedMemoryReadBuffer = new byte[_sharedmemorysize];
                _isSharedMemoryInitialised = true;
                RaiseConnectedEvent();
                StartDaemon();
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        internal TimeSpan SessionTime
        {
            get => _sessionTime;
            set => _sessionTime = value;
        }

        internal uint LastSessionState
        {
            get => _lastSessionState;
            set => _lastSessionState = value;
        }

        private void AsynConnector()
        {
            while (!TryConnect())
            {
                Thread.Sleep(10);
            }
        }

        internal Dictionary<string, DriverInfo> PreviousTickInfo
        {
            get => _previousTickInfo;
            set => _previousTickInfo = value;
        }

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
                    //This a state that sometimes occurs when saving pit preset during race
                    //This state is ignored, otherwise it would trigger a session reset
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
                        _previousTickInfo.Clear();
                        _nextSpeedComputation = 0;
                        RaiseSessionStartedEvent(simData);
                    }

                    _lastSessionState = data.MSessionState;
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
                catch (PCarsConnector.NameNotFilledException)
                {
                    //Ignore, names are sometimes not set in the shared memory
                }
            }

            _sharedMemory = null;
            RaiseDisconnectedEvent();
        }
       

        private bool ShouldResetSession(PCarsApiStruct data)
        {
            if (_lastSessionState != data.MSessionState)
            {
                return true;
            }
            if (this._previousSet.SessionInfo.SessionTimeRemaining - data.MEventTimeRemaining < -5)
            {
                return true;
            }
            return false;
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



        private void Disconnect()
        {
            _disconnect = true;

            _sharedMemory = null;
            RaiseDisconnectedEvent();
        }

        private PCarsApiStruct Load()
        {
            lock (_sharedMemory)
            {
                PCarsApiStruct pcarsapistruct = new PCarsApiStruct();
                if (!IsConnected)
                    throw new InvalidOperationException("Not connected");

                using (var sharedMemoryStreamView = _sharedMemory.CreateViewStream())
                {
                    BinaryReader sharedMemoryStream = new BinaryReader(sharedMemoryStreamView);
                    _sharedMemoryReadBuffer = sharedMemoryStream.ReadBytes(_sharedmemorysize);
                    _handle = GCHandle.Alloc(_sharedMemoryReadBuffer, GCHandleType.Pinned);
                    pcarsapistruct = (PCarsApiStruct)Marshal.PtrToStructure(_handle.AddrOfPinnedObject(), typeof(PCarsApiStruct));
                    _handle.Free();
                }

                return pcarsapistruct;
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
            _sessionTime = new TimeSpan(0, 0, 1);
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
    }
}
