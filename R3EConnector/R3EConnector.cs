namespace SecondMonitor.R3EConnector
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.InteropServices;
    using System.Threading;

    using SecondMonitor.DataModel;
    using SecondMonitor.DataModel.Drivers;
    using SecondMonitor.PluginManager.GameConnector;

    public class R3EConnector : IGameConnector
    {
        private static readonly string[] R3EExecutables = { "RRRE", "RRRE64" };
        private static readonly string SharedMemoryName = "$R3E";
        private readonly Queue<SimulatorDataSet> _queue = new Queue<SimulatorDataSet>();
        private MemoryMappedFile _sharedMemory;
        private Thread _daemonThread;
        private bool _disconnect;
        private bool _inSession;
        private int _lastSessionType;
        private int _lastSessionPhase;
        private Dictionary<string, DriverInfo> _lastTickInformation;

        private TimeSpan _sessionTime;
        private double _sessionStartR3RTime;
        private Process _process;        
        

        public event EventHandler<DataEventArgs> DataLoaded;

        public event EventHandler<DataEventArgs> SessionStarted;

        public event EventHandler<EventArgs> ConnectedEvent;

        public event EventHandler<EventArgs> Disconnected;

        private R3EDataConvertor DataConvertor { get; set; }



        public R3EConnector()
        {
            DataConvertor = new R3EDataConvertor(this);
            TickTime = 10;
            ResetConnector();
        }

        private void ResetConnector()
        {
            _inSession = false;            
            _lastTickInformation = new Dictionary<string, DriverInfo>();
            _lastSessionType = -2;
            _sessionTime = new TimeSpan(0);
            _sessionStartR3RTime = 0;
        }

        public bool IsConnected => (_sharedMemory != null);

        public int TickTime
        {
            get;
            set;
        }

        public bool IsR3ERunning()
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
            foreach (var processName in R3EExecutables)
            {
                var processes = Process.GetProcessesByName(processName);
                if (processes.Length <= 0)
                {
                    continue;
                }
                _process = processes[0];
                return true;
            }
            return false;
        }

        internal TimeSpan SessionTime => _sessionTime;

        public void ASyncConnect()
        {
            Thread asyncConnectThread = new Thread(AsyncConnector) { IsBackground = true };
            asyncConnectThread.Start();
        }

        public bool TryConnect()
        {
            return Connect();
        }

        private bool Connect()
        {
            if (!IsR3ERunning())
            {
                return false;
            }
            try
            {
                _sharedMemory = MemoryMappedFile.OpenExisting(SharedMemoryName);
                RaiseConnectedEvent();
                StartDaemon();
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        private void AsyncConnector()
        {
            while (!TryConnect())
            {
                Thread.Sleep(10);
            }
        }

        private void StartDaemon()
        {
            if (_daemonThread != null && _daemonThread.IsAlive)
            {
                throw new InvalidOperationException("Daemon is already running");
            }        
            ResetConnector();
            _disconnect = false;
            _queue.Clear();
            _daemonThread = new Thread(DaemonMethod) { IsBackground = true };
            _daemonThread.Start();

            Thread queueProcessorThread = new Thread(QueueProcessor) { IsBackground = true };
            queueProcessorThread.Start();

        }

        private void DaemonMethod()
        {

            while (!_disconnect)
            {
                Thread.Sleep(TickTime);
                R3ESharedData r3RData = Load();
                SimulatorDataSet data = DataConvertor.FromR3EData(r3RData);
                if (CheckSessionStarted(r3RData))
                {
                    _lastTickInformation.Clear();
                    RaiseSessionStartedEvent(data);
                }
                if (r3RData.GamePaused != 1 && r3RData.ControlType != (int) Constant.Control.Replay)
                {
                    _sessionTime = TimeSpan.FromSeconds(r3RData.Player.GameSimulationTime - _sessionStartR3RTime);
                }
                lock(_queue)
                {
                    _queue.Enqueue(data);
                }
                if (r3RData.ControlType == -1 && !IsR3ERunning())
                {
                    _disconnect = true;
                }
            }
            _sharedMemory = null;
            RaiseDisconnectedEvent();
        }

        private void QueueProcessor()
        {
            while (_disconnect == false)
            {
                SimulatorDataSet set;
                while (_queue.Count != 0)
                {
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

        internal void StoreLastTickInfo(DriverInfo driverInfo)
        {
            _lastTickInformation[driverInfo.DriverName] = driverInfo;
        }

        private void Disconnect()
        {
            _disconnect = true;
            _sharedMemory = null;
            RaiseDisconnectedEvent();
        }

        private R3ESharedData Load()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected");
            }
            MemoryMappedViewStream view = _sharedMemory.CreateViewStream();
            BinaryReader stream = new BinaryReader(view);
            byte[] buffer = stream.ReadBytes(Marshal.SizeOf(typeof(R3ESharedData)));
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            R3ESharedData data = (R3ESharedData)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(R3ESharedData));
            handle.Free();
            return data;
        }


        private bool CheckSessionStarted(R3ESharedData r3RData)
        {
            if (r3RData.SessionType != _lastSessionType)
            {
                _lastSessionType = r3RData.SessionType;
                _sessionStartR3RTime = r3RData.Player.GameSimulationTime;
                return true;
            }

            if (r3RData.SessionPhase != -1 && !_inSession)
            {
                _inSession = true;
                _sessionStartR3RTime = r3RData.Player.GameSimulationTime;
                return true;
            }

            if (_inSession && r3RData.SessionPhase == -1)
            {
                _inSession = false;
            }

            if (_lastSessionPhase >= 5 && r3RData.SessionPhase < 5 )
            {
                _lastSessionPhase = r3RData.SessionPhase;
                return true;
            }
            _lastSessionPhase = r3RData.SessionPhase;
            return false;
        }

        private void RaiseSessionStartedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            EventHandler<DataEventArgs> handler = SessionStarted;
            _sessionTime = new TimeSpan(0,0,1);
            handler?.Invoke(this, args);
        }

        private void RaiseDataLoadedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            DataLoaded?.Invoke(this, args);
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