namespace SecondMonitor.RF2Connector
{
    using System;
    using System.IO;
    using System.Threading;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.PluginManager.DependencyChecker;
    using SecondMonitor.PluginManager.GameConnector;
    using SecondMonitor.PluginManager.GameConnector.SharedMemory;
    using SecondMonitor.RF2Connector.SharedMemory;
    using SecondMonitor.RF2Connector.SharedMemory.rFactor2Data;

    // Based on https://github.com/TheIronWolfModding/rF2SharedMemoryMapPlugin
    internal class Rf2Connector : AbstractGameConnector
    {
        private static readonly string[] RFExecutables = { "rFactor2" };
        private readonly TimeSpan _connectionTimeout = TimeSpan.FromSeconds(120);
        private readonly RF2DataConvertor _rf2DataConvertor;
        private readonly MappedBuffer<rF2Telemetry> _telemetryBuffer = new MappedBuffer<rF2Telemetry>(rFactor2Constants.MM_TELEMETRY_FILE_NAME);
        private readonly MappedBuffer<rF2Scoring> _scoringBuffer = new MappedBuffer<rF2Scoring>(rFactor2Constants.MM_SCORING_FILE_NAME);
        private readonly MappedBuffer<rF2Rules> _rulesBuffer = new MappedBuffer<rF2Rules>(rFactor2Constants.MM_RULES_FILE_NAME);
        private readonly MappedBuffer<rF2Extended> _extendedBuffer = new MappedBuffer<rF2Extended>(rFactor2Constants.MM_EXTENDED_FILE_NAME);
        private readonly DependencyChecker dependencies;

        private DateTime _connectionTime = DateTime.MinValue;
        private int _rawLastSessionType = int.MinValue;
        private SessionPhase _lastSessionPhase;
        private SessionType _lastSessionType;
        private bool _isConnected;

        public Rf2Connector()
            : base(RFExecutables)
        {
            TickTime = 10;

            dependencies = new DependencyChecker(new FileExistDependency[]{ new FileExistDependency(@"Plugins\rFactor2SharedMemoryMapPlugin64.dll", @"Connectors\RFactor2\rFactor2SharedMemoryMapPlugin64.dll") }, () => true );
            _rf2DataConvertor = new RF2DataConvertor();
        }

        public override bool IsConnected => _isConnected;


        protected override void OnConnection()
        {
            ResetConnector();
            CheckDependencies();
            if (_connectionTime == DateTime.MinValue)
            {
                _connectionTime = DateTime.Now;
            }
            if (DateTime.Now - _connectionTime > _connectionTimeout)
            {
                SendMessageToClients(
                    "A rFactor2 based game has been detected running for extended time, but SecondMonitor wasn't able to connect to its shared memory.\n"
                    + "Please make sure that the rFactor2SharedMemoryMapPlugin64.dll is correctly installed in the plugins folder and enabled");
                _connectionTime = DateTime.MaxValue;
            }

            try
            {
                _telemetryBuffer.Connect();
                _scoringBuffer.Connect();
                _rulesBuffer.Connect();
                _extendedBuffer.Connect();
                _isConnected = true;
            }
            catch (Exception)
            {
                Disconnect();
                throw;
            }
        }

        private void CheckDependencies()
        {
            if(Process != null && !dependencies.Checked)
            {
                string directory = Path.Combine(Path.GetPathRoot(Process.MainModule.FileName), Path.GetDirectoryName(Process.MainModule.FileName));
                Action actionToInstall = dependencies.CheckAndReturnInstallDependenciesAction(directory);
                if (actionToInstall != null)
                {
                    SendMessageToClients("A rFactor2 based game has been detected, but the required plugin, rFactor2SharedMemoryMapPlugin64.dll, was not found. Do you want Second Monitor to install this plugin? You will need to restart the sim, after it is done.",
                        () => RunActionAndShowConfirmation(actionToInstall, "Operation Completed Successfully",  "Unable to install the plugin, unexpected error: "));
                }
            }
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

        private void Disconnect()
        {
            _extendedBuffer.Disconnect();
            _scoringBuffer.Disconnect();
            _rulesBuffer.Disconnect();
            _telemetryBuffer.Disconnect();
            _isConnected = false;
        }

        protected override void ResetConnector()
        {
            _rawLastSessionType = int.MinValue;
            _lastSessionType = SessionType.Na;
            _lastSessionPhase = SessionPhase.Countdown;
        }

        protected override string ConnectorName => "RFactor2";

        protected override void DaemonMethod()
        {
            _connectionTime = DateTime.MinValue;
            while (!ShouldDisconnect)
            {
                Thread.Sleep(TickTime);
                Rf2FullData rFactorData = Load();
                SimulatorDataSet dataSet;
                try
                {
                    dataSet = _rf2DataConvertor.CreateSimulatorDataSet(rFactorData);
                }
                catch (RF2InvalidPackageException)
                {
                    continue;
                }

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
            Disconnect();
            RaiseDisconnectedEvent();
        }

        private Rf2FullData Load()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected");
            }

            Rf2FullData data = new Rf2FullData()
                                   {
                                       extended = _extendedBuffer.GetMappedDataUnSynchronized(),
                                       scoring = _scoringBuffer.GetMappedDataUnSynchronized(),
                                       telemetry = _telemetryBuffer.GetMappedDataUnSynchronized(),
                                       rules = _rulesBuffer.GetMappedDataUnSynchronized()
                                   };
            return data;
        }

        private bool CheckSessionStarted(Rf2FullData rfData, SimulatorDataSet dataSet)
        {
            if (_rawLastSessionType != rfData.scoring.mScoringInfo.mSession || _lastSessionType != dataSet.SessionInfo.SessionType)
            {
                _lastSessionType = dataSet.SessionInfo.SessionType;
                _rawLastSessionType = rfData.scoring.mScoringInfo.mSession;
                _lastSessionPhase = dataSet.SessionInfo.SessionPhase;
                return true;
            }

            if (dataSet.SessionInfo.SessionPhase != _lastSessionPhase && _lastSessionPhase != SessionPhase.Green && dataSet.SessionInfo.SessionPhase != SessionPhase.Countdown)
            {
                _lastSessionType = dataSet.SessionInfo.SessionType;
                _rawLastSessionType = rfData.scoring.mScoringInfo.mSession;
                _lastSessionPhase = dataSet.SessionInfo.SessionPhase;
                return true;
            }
            return false;
        }
    }
}