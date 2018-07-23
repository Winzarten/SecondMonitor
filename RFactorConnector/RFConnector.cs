namespace SecondMonitor.RFactorConnector
{
    using System;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.InteropServices;
    using System.Threading;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.PluginManager.GameConnector;
    using SecondMonitor.RFactorConnector.SharedMemory;

    public class RFConnector : AbstractGameConnector
    {
        private static readonly string[] RFExecutables = { "AMS", "rFactor", "GSC" };
        private static readonly string SharedMemoryName = "$rFactorShared$";
        private readonly RFDataConvertor _rfDataConvertor;

        private DateTime _connectionTime = DateTime.MinValue;

        private TimeSpan _connectionTimeout = TimeSpan.FromSeconds(20);

        private MemoryMappedFile _sharedMemory;
        private int _rawLastSessionType = int.MinValue;

        private SessionPhase _lastSessionPhase;

        private SessionType _lastSessionType;

        public RFConnector()
            : base(RFExecutables)
        {
            TickTime = 10;
            _rfDataConvertor = new RFDataConvertor();
        }

        public override bool IsConnected { get => _sharedMemory != null; }


        protected override void OnConnection()
        {
            ResetConnector();
            if (_connectionTime == DateTime.MinValue)
            {
                _connectionTime = DateTime.Now;
            }
            if (DateTime.Now - _connectionTime > _connectionTimeout)
            {
                SendMessageToClients("A rFactor based game has been detected running for extended time, but SecondMonitor wasn't able to connect to its shared memory.\n"
                                     + "Please make sure that the rFactorSharedMemoryMap.dll is correctly installed in the plugins folder.\n"
                                     + "The plugin can be downloaded at : https://github.com/dallongo/rFactorSharedMemoryMap/releases ");
                _connectionTime = DateTime.MaxValue;
            }
            _sharedMemory = MemoryMappedFile.OpenExisting(SharedMemoryName);
        }

        protected override void ResetConnector()
        {
            _rawLastSessionType = int.MinValue;
            _lastSessionType = SessionType.Na;
            _lastSessionPhase = SessionPhase.Countdown;
        }

        protected override string ConnectorName => "RFactor";

        protected override void DaemonMethod()
        {
            _connectionTime = DateTime.MinValue;
            while (!ShouldDisconnect)
            {
                Thread.Sleep(TickTime);
                RfShared rFactorData = Load();

                SimulatorDataSet dataSet = _rfDataConvertor.CreateSimulatorDataSet(rFactorData);

                if (CheckSessionStarted(rFactorData, dataSet))
                {
                    RaiseSessionStartedEvent(dataSet);
                }

                RaiseDataLoadedEvent(dataSet);

                if (!IsProcessRunning())
                {
                    ShouldDisconnect = true;
                }
            }
            _sharedMemory = null;
            RaiseDisconnectedEvent();
        }

        private RfShared Load()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected");
            }
            using (var view = _sharedMemory.CreateViewStream())
            {
                BinaryReader stream = new BinaryReader(view);
                byte[] buffer = stream.ReadBytes(Marshal.SizeOf(typeof(RfShared)));
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                RfShared data = (RfShared)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(RfShared));
                handle.Free();
                return data;
            }
        }

        private bool CheckSessionStarted(RfShared rfData, SimulatorDataSet dataSet)
        {
            if (_rawLastSessionType != rfData.Session || _lastSessionType != dataSet.SessionInfo.SessionType)
            {
                _lastSessionType = dataSet.SessionInfo.SessionType;
                _rawLastSessionType = rfData.Session;
                _lastSessionPhase = dataSet.SessionInfo.SessionPhase;
                return true;
            }

            if (dataSet.SessionInfo.SessionPhase != _lastSessionPhase && _lastSessionPhase != SessionPhase.Green && dataSet.SessionInfo.SessionPhase != SessionPhase.Countdown)
            {
                _lastSessionType = dataSet.SessionInfo.SessionType;
                _rawLastSessionType = rfData.Session;
                _lastSessionPhase = dataSet.SessionInfo.SessionPhase;
                return true;
            }
            return false;
        }
    }
}