using System;
using System.Threading;
using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.DataModel.Snapshot;
using SecondMonitor.PluginManager.GameConnector;
using SecondMonitor.RF2Connector.SharedMemory;
using SecondMonitor.RF2Connector.SharedMemory.rFactor2Data;

namespace SecondMonitor.RF2Connector
{
    //Based on https://github.com/TheIronWolfModding/rF2SharedMemoryMapPlugin
    internal class Rf2Connector : AbstractGameConnector
    {
        private static readonly string[] RFExecutables = { "rFactor2" };
        private readonly TimeSpan _connectionTimeout = TimeSpan.FromSeconds(120);
        private readonly RF2DataConvertor _rf2DataConvertor;
        private readonly MappedBuffer<rF2Telemetry> _telemetryBuffer = new MappedBuffer<rF2Telemetry>(rFactor2Constants.MM_TELEMETRY_FILE_NAME, true /*partial*/, true /*skipUnchanged*/);
        private readonly MappedBuffer<rF2Scoring> _scoringBuffer = new MappedBuffer<rF2Scoring>(rFactor2Constants.MM_SCORING_FILE_NAME, true /*partial*/, true /*skipUnchanged*/);
        private readonly MappedBuffer<rF2Rules> _rulesBuffer = new MappedBuffer<rF2Rules>(rFactor2Constants.MM_RULES_FILE_NAME, true /*partial*/, true /*skipUnchanged*/);
        private readonly MappedBuffer<rF2Extended> _extendedBuffer = new MappedBuffer<rF2Extended>(rFactor2Constants.MM_EXTENDED_FILE_NAME, false /*partial*/, true /*skipUnchanged*/);

        private DateTime _connectionTime = DateTime.MinValue;
        private int _rawLastSessionType = int.MinValue;
        private SessionPhase _lastSessionPhase;
        private SessionType _lastSessionType;
        private bool _isConnected;

        public Rf2Connector()
            : base(RFExecutables)
        {
            TickTime = 10;
            _rf2DataConvertor = new RF2DataConvertor();
        }

        public override bool IsConnected => _isConnected;


        protected override void OnConnection()
        {
            ResetConnector();
            if (_connectionTime == DateTime.MinValue)
            {
                _connectionTime = DateTime.Now;
            }
            if (DateTime.Now - _connectionTime > _connectionTimeout)
            {
                SendMessageToClients("A rFactor2 based game has been detected running for extended time, but SecondMonitor wasn't able to connect to its shared memory.\n"
                                     + "Please make sure that the rFactorSharedMemoryMap.dll is correctly installed in the plugins folder.\n"
                                     + "The plugin can be downloaded at : https://github.com/dallongo/rFactorSharedMemoryMap/releases ");
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

            Rf2FullData data = new Rf2FullData();
            rF2Extended rF2Extended = new rF2Extended();
            rF2Scoring rF2Scoring = new rF2Scoring();
            rF2Telemetry rF2Telemetry = new rF2Telemetry();
            rF2Rules rF2Rules = new rF2Rules();

            _extendedBuffer.GetMappedData(ref rF2Extended);
            _scoringBuffer.GetMappedData(ref rF2Scoring);
            _telemetryBuffer.GetMappedData(ref rF2Telemetry);
            _rulesBuffer.GetMappedData(ref rF2Rules);

            data.extended = rF2Extended;
            data.scoring = rF2Scoring;
            data.telemetry = rF2Telemetry;
            data.rules = rF2Rules;
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