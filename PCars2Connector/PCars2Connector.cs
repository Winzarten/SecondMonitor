namespace SecondMonitor.PCars2Connector
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.PCars2Connector.DataConvertor;
    using SecondMonitor.PCars2Connector.SharedMemory;
    using SecondMonitor.PluginManager.GameConnector;
    using SecondMonitor.PluginManager.GameConnector.SharedMemory;

    public class PCars2Connector : AbstractGameConnector
    {
        private static readonly string[] PCars2Executables = { "pCARS2", "pCARS2AVX" };
        private static readonly string SharedMemoryName = "$pcars2$";

        private readonly TimeSpan _connectionTimeout = TimeSpan.FromSeconds(120);
        private readonly MappedBuffer<PCars2SharedMemory> _sharedMemory;
        private readonly PCars2DataConvertor _pCars2DataConvertor;
        private readonly Stopwatch _stopwatch;


        private bool _isConnected;
        private DateTime _connectionTime = DateTime.MinValue;
        private SimulatorDataSet _lastDataSet;

        private PCars2SessionType _lastRawPCars2SessionType;
        private SessionType _lastSessionType;
        private SessionPhase _lastSessionPhase;

        public PCars2Connector()
            : base(PCars2Executables)
        {

            _sharedMemory = new MappedBuffer<PCars2SharedMemory>(SharedMemoryName);

            _pCars2DataConvertor = new PCars2DataConvertor();
            _lastRawPCars2SessionType = PCars2SessionType.SessionInvalid;
            _lastSessionType = SessionType.Na;
            _lastSessionPhase = SessionPhase.Countdown;
            _stopwatch = new Stopwatch();
        }

        public override bool IsConnected => _isConnected;

        protected internal TimeSpan SessionTime => _stopwatch?.Elapsed ?? TimeSpan.Zero;

        protected override string ConnectorName => "PCars 2";

        protected override void OnConnection()
        {
            ResetConnector();

            if (_connectionTime == DateTime.MinValue)
            {
                _connectionTime = DateTime.Now;
            }

            try
            {
                _sharedMemory.Connect();
                _isConnected = true;
            }
            catch (Exception)
            {
                Disconnect();
                throw;
            }
        }

        protected override void ResetConnector()
        {
            _stopwatch.Restart();
            _lastRawPCars2SessionType = PCars2SessionType.SessionInvalid;
            _lastSessionType = SessionType.Na;
            _lastSessionPhase = SessionPhase.Countdown;
            _lastDataSet = null;
        }

        protected override void DaemonMethod()
        {
            _connectionTime = DateTime.MinValue;
            while (!ShouldDisconnect)
            {
                Thread.Sleep(TickTime);
                PCars2SharedMemory rawData = ReadAllBuffers();

                if (!_stopwatch.IsRunning && ((GameState)rawData.mGameState == GameState.GameInGamePlaying || (GameState)rawData.mGameState == GameState.GameInGameInMenuTimeTicking))
                {
                    _stopwatch.Start();
                }

                if (_stopwatch.IsRunning && ((GameState)rawData.mGameState != GameState.GameInGamePlaying && (GameState)rawData.mGameState != GameState.GameInGameInMenuTimeTicking))
                {
                    _stopwatch.Stop();
                }

                SimulatorDataSet dataSet = _pCars2DataConvertor.CreateSimulatorDataSet(rawData, TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds));

                if (CheckSessionStarted(rawData, dataSet))
                {
                    _stopwatch.Restart();
                    dataSet.SessionInfo.SessionTime = _stopwatch.Elapsed;
                    RaiseSessionStartedEvent(dataSet);
                }

                RaiseDataLoadedEvent(dataSet);
                _lastDataSet = dataSet;

                if (!IsProcessRunning())
                {
                    ShouldDisconnect = true;
                }
            }
            Disconnect();
            RaiseDisconnectedEvent();
        }

        private PCars2SharedMemory ReadAllBuffers()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected");
            }

            return _sharedMemory.GetMappedDataUnSynchronized();
        }


        private void Disconnect()
        {
            _sharedMemory.Disconnect();
            _isConnected = false;
        }


        private bool CheckSessionStarted(PCars2SharedMemory rawData, SimulatorDataSet dataSet)
        {
            if (_lastRawPCars2SessionType != (PCars2SessionType)rawData.mSessionState || _lastSessionType != dataSet.SessionInfo.SessionType)
            {
                _lastSessionType = dataSet.SessionInfo.SessionType;
                _lastRawPCars2SessionType = (PCars2SessionType)rawData.mSessionState;
                _lastSessionPhase = dataSet.SessionInfo.SessionPhase;
                return true;
            }

            return false;
        }
    }
}