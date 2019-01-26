namespace SecondMonitor.PCars2Connector
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataConvertor;
    using SharedMemory;
    using PluginManager.GameConnector;
    using SecondMonitor.PluginManager.GameConnector.SharedMemory;

    public class PCars2Connector : AbstractGameConnector
    {
        private static readonly string[] PCars2Executables = { "pCARS2", "pCARS2AVX" };
        private static readonly string SharedMemoryName = "$pcars2$";

        private readonly MappedBuffer<PCars2SharedMemory> _sharedMemory;
        private readonly PCars2DataConvertor _pCars2DataConvertor;
        private readonly Stopwatch _stopwatch;


        private bool _isConnected;


        private PCars2SessionType _lastRawPCars2SessionType;
        private SessionType _lastSessionType;

        public PCars2Connector()
            : base(PCars2Executables)
        {

            _sharedMemory = new MappedBuffer<PCars2SharedMemory>(SharedMemoryName);

            _pCars2DataConvertor = new PCars2DataConvertor();
            _lastRawPCars2SessionType = PCars2SessionType.SessionInvalid;
            _lastSessionType = SessionType.Na;
            _stopwatch = new Stopwatch();
        }

        public override bool IsConnected => _isConnected;

        protected internal TimeSpan SessionTime => _stopwatch?.Elapsed ?? TimeSpan.Zero;

        protected override string ConnectorName => "PCars 2";

        protected override void OnConnection()
        {
            ResetConnector();

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
        }

        protected override async Task DaemonMethod()
        {
            while (!ShouldDisconnect)
            {
                await Task.Delay(TickTime).ConfigureAwait(false);
                PCars2SharedMemory rawData = ReadAllBuffers();

                if (!_stopwatch.IsRunning && ((GameState)rawData.mGameState == GameState.GameInGamePlaying || (GameState)rawData.mGameState == GameState.GameInGameInMenuTimeTicking))
                {
                    _stopwatch.Start();
                }

                if (_stopwatch.IsRunning && ((GameState)rawData.mGameState != GameState.GameInGamePlaying && (GameState)rawData.mGameState != GameState.GameInGameInMenuTimeTicking))
                {
                    _stopwatch.Stop();
                }

                // This a state that sometimes occurs when saving pit preset during race
                // This state is ignored, otherwise it would trigger a session reset
                if (rawData.mSessionState == 0 && (rawData.mGameState == 2 || rawData.mGameState == 3))
                {
                    continue;
                }

                SimulatorDataSet dataSet = _pCars2DataConvertor.CreateSimulatorDataSet(rawData, TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds));

                if (CheckSessionStarted(rawData, dataSet))
                {
                    _stopwatch.Restart();
                    dataSet.SessionInfo.SessionTime = _stopwatch.Elapsed;
                    RaiseSessionStartedEvent(dataSet);
                }

                RaiseDataLoadedEvent(dataSet);

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
                return true;
            }

            return false;
        }
    }
}