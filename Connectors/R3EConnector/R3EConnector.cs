namespace SecondMonitor.R3EConnector
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using DataModel.Snapshot;
    using PluginManager.GameConnector;
    using DriverInfo = DataModel.Snapshot.Drivers.DriverInfo;

    public class R3EConnector : AbstractGameConnector
    {
        private static readonly string[] R3EExecutables = { "RRRE", "RRRE64" };
        private static readonly string SharedMemoryName = "$R3E";
        private MemoryMappedFile _sharedMemory;
        private bool _inSession;
        private int _lastSessionType;
        private int _lastSessionPhase;
        private Dictionary<string, DriverInfo> _lastTickInformation;

        private TimeSpan _sessionTime;
        private TimeSpan _lastSessionTime = TimeSpan.Zero;

        private DateTime _startSessionTime;
        private double _sessionStartR3RTime;

       private R3EDataConvertor DataConvertor { get; set; }

        public R3EConnector() : base(R3EExecutables)
        {
            DataConvertor = new R3EDataConvertor(this);
            TickTime = 16;
        }

        protected override void ResetConnector()
        {
            _inSession = false;
            _lastTickInformation = new Dictionary<string, DriverInfo>();
            _lastSessionType = -2;
            _sessionTime = TimeSpan.Zero;
            _sessionStartR3RTime = 0;
            _lastSessionTime = TimeSpan.Zero;
        }

        public override bool IsConnected => _sharedMemory != null;

        internal TimeSpan SessionTime => _sessionTime;

        protected override string ConnectorName => "R3E";

        protected override void OnConnection()
        {
            ResetConnector();
            _sharedMemory = MemoryMappedFile.OpenExisting(SharedMemoryName);
        }

        protected override async Task DaemonMethod(CancellationToken cancellationToken)
        {

            while (!ShouldDisconnect)
            {
                await Task.Delay(TickTime, cancellationToken).ConfigureAwait(false);
                R3ESharedData r3RData = Load();
                SimulatorDataSet data = DataConvertor.FromR3EData(r3RData);
                if (CheckSessionStarted(r3RData, data))
                {
                    _lastTickInformation.Clear();
                    _sessionTime = new TimeSpan(0, 0, 1);
                    _lastSessionTime = _sessionTime;
                    RaiseSessionStartedEvent(data);
                }

                if (r3RData.GamePaused != 1 && r3RData.ControlType != (int) Constant.Control.Replay)
                {
                    if (r3RData.Player.GameSimulationTime != 0)
                    {
                        _sessionTime = TimeSpan.FromSeconds(r3RData.Player.GameSimulationTime - _sessionStartR3RTime);
                    }
                    else
                    {
                        _sessionTime = DateTime.UtcNow - _startSessionTime;
                    }
                }

                _lastSessionTime = data.SessionInfo.SessionTime;
                AddToQueue(data);

                if (r3RData.ControlType == -1 && !IsProcessRunning())
                {
                    ShouldDisconnect = true;
                }
            }

            _sharedMemory = null;
            _sessionTime = new TimeSpan(0, 0, 1);
            RaiseDisconnectedEvent();
        }

        internal void StoreLastTickInfo(DriverInfo driverInfo)
        {
            _lastTickInformation[driverInfo.DriverName] = driverInfo;
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


        private bool CheckSessionStarted(R3ESharedData r3RData, SimulatorDataSet data)
        {
            if (r3RData.SessionType != _lastSessionType)
            {
                _lastSessionType = r3RData.SessionType;
                _sessionStartR3RTime = r3RData.Player.GameSimulationTime;
                _startSessionTime = DateTime.UtcNow;
                return true;
            }

            if (r3RData.SessionPhase != -1 && !_inSession)
            {
                _inSession = true;
                _sessionStartR3RTime = r3RData.Player.GameSimulationTime;
                _startSessionTime = DateTime.UtcNow;
                return true;
            }

            if (_inSession && r3RData.SessionPhase == -1)
            {
                _sessionStartR3RTime = r3RData.Player.GameSimulationTime;
                _startSessionTime = DateTime.UtcNow;
                _inSession = false;
            }

            if (_lastSessionPhase >= 5 && r3RData.SessionPhase < 5 )
            {
                _sessionStartR3RTime = r3RData.Player.GameSimulationTime;
                _startSessionTime = DateTime.UtcNow;
                _lastSessionPhase = r3RData.SessionPhase;
                return true;
            }

            if (data.SessionInfo.SessionTime < _lastSessionTime)
            {
                _sessionStartR3RTime = r3RData.Player.GameSimulationTime;
                _startSessionTime = DateTime.UtcNow;
                return true;
            }

            _lastSessionPhase = r3RData.SessionPhase;
            return false;
        }
    }
}