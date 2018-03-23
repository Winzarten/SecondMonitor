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
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;
    using SecondMonitor.PluginManager.GameConnector;

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
        private double _sessionStartR3RTime;
        
       private R3EDataConvertor DataConvertor { get; set; }

        public R3EConnector() : base(R3EExecutables)
        {
            DataConvertor = new R3EDataConvertor(this);
            TickTime = 10;
        }

        protected override void ResetConnector()
        {
            _inSession = false;
            _lastTickInformation = new Dictionary<string, DriverInfo>();
            _lastSessionType = -2;
            _sessionTime = TimeSpan.Zero;
            _sessionStartR3RTime = 0;
        }

        public override bool IsConnected => _sharedMemory != null;

        internal TimeSpan SessionTime => _sessionTime;

        protected override void OnConnection()
        {
            this.ResetConnector();
            _sharedMemory = MemoryMappedFile.OpenExisting(SharedMemoryName);
        }

        protected override void DaemonMethod()
        {

            while (!this.ShouldDisconnect)
            {
                Thread.Sleep(TickTime);
                R3ESharedData r3RData = Load();
                SimulatorDataSet data = DataConvertor.FromR3EData(r3RData);
                if (CheckSessionStarted(r3RData))
                {
                    _lastTickInformation.Clear();
                    _sessionTime = new TimeSpan(0, 0, 1);
                    RaiseSessionStartedEvent(data);
                }

                if (r3RData.GamePaused != 1 && r3RData.ControlType != (int) Constant.Control.Replay)
                {
                    _sessionTime = TimeSpan.FromSeconds(r3RData.Player.GameSimulationTime - _sessionStartR3RTime);
                }

                AddToQueue(data);

                if (r3RData.ControlType == -1 && !this.IsProcessRunning())
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
    }
}