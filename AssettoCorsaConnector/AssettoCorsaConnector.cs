namespace SecondMonitor.AssettoCorsaConnector
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using SecondMonitor.AssettoCorsaConnector.SharedMemory;
    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.PluginManager.DependencyChecker;
    using SecondMonitor.PluginManager.GameConnector;
    using SecondMonitor.PluginManager.GameConnector.SharedMemory;

    public class AssettoCorsaConnector : AbstractGameConnector
    {
        private static readonly string[] ACExecutables = { "acs_x86", "acs" };
        private readonly DependencyChecker dependencies;
        private readonly TimeSpan _connectionTimeout = TimeSpan.FromSeconds(120);
        private readonly MappedBuffer<SPageFilePhysics> _physicsBuffer;
        private readonly MappedBuffer<SPageFileGraphic> _graphicsBuffer;
        private readonly MappedBuffer<SPageFileStatic> _staticBuffer;
        private readonly MappedBuffer<SPageFileSecondMonitor> _secondMonitorBuffer;
        private readonly AcDataConverter _acDataConverter;

        private readonly Stopwatch _stopwatch;

        private AcSessionType _rawLastSessionType;
        private SessionType _lastSessionType;
        private SessionPhase _lastSessionPhase;

        private bool _isConnected;
        private DateTime _connectionTime = DateTime.MinValue;
        private SimulatorDataSet _lastDataSet;

        public AssettoCorsaConnector()
            : base(ACExecutables)
        {
            dependencies = new DependencyChecker(
                new IDependency[]
                    {
                        new DirectoryExistsDependency(@"apps\python\SecondMonitor"),
                        new FileExistsAndMatchDependency(
                            @"apps\python\SecondMonitor\SecondMonitor.py",
                            @"Connectors\AssettoCorsa\SecondMonitor.py"),
                        new FileExistsAndMatchDependency(
                            @"apps\python\SecondMonitor\smshared_mem.py",
                            @"Connectors\AssettoCorsa\smshared_mem.py")
                    },
                () => true);

            _physicsBuffer = new MappedBuffer<SPageFilePhysics>(AssettoCorsaShared.SharedMemoryNamePhysics);
            _graphicsBuffer = new MappedBuffer<SPageFileGraphic>(AssettoCorsaShared.SharedMemoryNameGraphic);
            _staticBuffer = new MappedBuffer<SPageFileStatic>(AssettoCorsaShared.SharedMemoryNameStatic);
            _secondMonitorBuffer = new MappedBuffer<SPageFileSecondMonitor>(AssettoCorsaShared.SharedMemoryNameSecondMonitor);
            _acDataConverter = new AcDataConverter(this);
            _rawLastSessionType = AcSessionType.AC_UNKNOWN;
            _lastSessionType = SessionType.Na;
            _lastSessionPhase = SessionPhase.Countdown;
            _stopwatch = new Stopwatch();
        }

        public override bool IsConnected => _isConnected;

        protected override string ConnectorName  => "Assetto Corsa";

        protected internal TimeSpan SessionTime => _stopwatch?.Elapsed ?? TimeSpan.Zero;

        protected override void OnConnection()
        {
            ResetConnector();
            CheckDependencies();
            if (_connectionTime == DateTime.MinValue)
            {
                _connectionTime = DateTime.Now;
            }

            try
            {
                _physicsBuffer.Connect();
                _graphicsBuffer.Connect();
                _staticBuffer.Connect();
                _secondMonitorBuffer.Connect();
                _isConnected = true;
            }
            catch (Exception)
            {
                Disconnect();
                throw;
            }
        }

        private void Disconnect()
        {
            _physicsBuffer.Disconnect();
            _graphicsBuffer.Disconnect();
            _staticBuffer.Disconnect();
            _secondMonitorBuffer.Disconnect();
            _isConnected = false;
        }

        protected override void ResetConnector()
        {
            _stopwatch.Restart();
            _rawLastSessionType = AcSessionType.AC_UNKNOWN;
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
                AssettoCorsaShared acData = ReadAllBuffers();

                if (!_stopwatch.IsRunning && acData.AcsGraphic.status == AcStatus.AC_LIVE)
                {
                    _stopwatch.Start();
                }

                if (_stopwatch.IsRunning && acData.AcsGraphic.status != AcStatus.AC_LIVE)
                {
                    _stopwatch.Stop();
                }

                SimulatorDataSet dataSet = _acDataConverter.CreateSimulatorDataSet(acData);

                if (CheckSessionStarted(acData, dataSet))
                {
                    _stopwatch.Restart();
                    dataSet.SessionInfo.SessionTime = _stopwatch.Elapsed;
                    _acDataConverter.ResetConverter();
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

        private AssettoCorsaShared ReadAllBuffers()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected");
            }

            AssettoCorsaShared data = new AssettoCorsaShared()
            {
                AcsGraphic = _graphicsBuffer.GetMappedDataUnSynchronized(),
                AcsPhysics = _physicsBuffer.GetMappedDataUnSynchronized(),
                AcsSecondMonitor = _secondMonitorBuffer.GetMappedDataUnSynchronized(),
                AcsStatic = _staticBuffer.GetMappedDataUnSynchronized()
            };

            return data;
        }

        private void CheckDependencies()
        {
            if (Process != null && !dependencies.Checked)
            {
                string directory = Path.Combine(Path.GetPathRoot(Process.MainModule.FileName), Path.GetDirectoryName(Process.MainModule.FileName));
                Action actionToInstall = dependencies.CheckAndReturnInstallDependenciesAction(directory);
                if (actionToInstall != null)
                {
                    SendMessageToClients(
                        "Assetto Corsa has been detected, but the required second monitor plugin, was not found, or is outdated. Do you want Second Monitor to install this plugin? You will need to restart the sim, after it is done.",
                        () => RunActionAndShowConfirmation(
                            actionToInstall,
                            "Operation Completed Successfully",
                            "Unable to install the plugin, unexpected error: "));
                }
            }
        }

        private bool CheckSessionStarted(AssettoCorsaShared acData, SimulatorDataSet dataSet)
        {
            if (_rawLastSessionType != acData.AcsGraphic.session || _lastSessionType != dataSet.SessionInfo.SessionType)
            {
                _lastSessionType = dataSet.SessionInfo.SessionType;
                _rawLastSessionType = acData.AcsGraphic.session;
                _lastSessionPhase = dataSet.SessionInfo.SessionPhase;
                return true;
            }

            if (dataSet.SessionInfo.SessionPhase != _lastSessionPhase && _lastSessionPhase != SessionPhase.Green
                                                                      && dataSet.SessionInfo.SessionPhase
                                                                      != SessionPhase.Countdown)
            {
                _lastSessionType = dataSet.SessionInfo.SessionType;
                _rawLastSessionType = acData.AcsGraphic.session;
                _lastSessionPhase = dataSet.SessionInfo.SessionPhase;
                return true;
            }

            if (_lastDataSet == null)
            {
                return false;
            }

            if (_lastDataSet.PlayerInfo?.CompletedLaps > dataSet.PlayerInfo?.CompletedLaps)
            {
                return true;
            }

            if (_lastDataSet.SessionInfo.SessionType == SessionType.Race && _lastDataSet.SessionInfo.LeaderCurrentLap
                > dataSet.SessionInfo.LeaderCurrentLap)
            {
                return true;
            }


            return false;
        }

        private void RunActionAndShowConfirmation(Action actionToRun, string completionMessage, string errorMessage)
        {
            try
            {
                actionToRun();
                SendMessageToClients(completionMessage);
            }
            catch (Exception ex)
            {
                SendMessageToClients(errorMessage + "\n" + ex.Message);
            }
        }

        
    }
}