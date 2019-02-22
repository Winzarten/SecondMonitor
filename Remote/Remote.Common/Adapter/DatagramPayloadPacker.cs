using NLog;

namespace SecondMonitor.Remote.Common.Adapter
{
    using System;
    using System.Diagnostics;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;
    using Model;
    using PluginsConfiguration.Common.Controller;
    using PluginsConfiguration.Common.DataModel;

    public class DatagramPayloadPacker : IDatagramPayloadPacker
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Random _random;

        private string _lastSimulatorSourceName;
        private readonly bool _isNetworkConservationEnabled;
        private readonly TimeSpan _packedDelay;
        private readonly TimeSpan _playerInfoDelay;
        private readonly TimeSpan _driversInfoDelay;

        private readonly Stopwatch _packageTimer;
        private readonly Stopwatch _playerInfoDelayTimer;
        private readonly Stopwatch _driversInfoDelayTimer;
        private SimulatorDataSet _lastDataSet;

        public DatagramPayloadPacker(IPluginSettingsProvider pluginSettingsProvider)
        {
            BroadcastLimitSettings broadcastLimitSettings = pluginSettingsProvider.RemoteConfiguration.BroadcastLimitSettings;
            _random = new Random();
            _isNetworkConservationEnabled = broadcastLimitSettings.IsEnabled;
            _packedDelay = TimeSpan.FromMilliseconds(broadcastLimitSettings.MinimumPackageInterval);
            _playerInfoDelay = TimeSpan.FromMilliseconds(broadcastLimitSettings.PlayerTimingPackageInterval);
            _driversInfoDelay = TimeSpan.FromMilliseconds(broadcastLimitSettings.OtherDriversTimingPackageInterval);



            if (!_isNetworkConservationEnabled)
            {
                Logger.Info("Network conservation is disabled");
                return;
            }

            Logger.Info("Network conservation is enabled");
            Logger.Info($"Package Delay {_packedDelay.TotalMilliseconds}");
            Logger.Info($"Player info Delay {_playerInfoDelay.TotalMilliseconds}");
            Logger.Info($"Other drivers info Delay {_driversInfoDelay.TotalMilliseconds}");

            _packageTimer = Stopwatch.StartNew();
            _playerInfoDelayTimer = Stopwatch.StartNew();
            _driversInfoDelayTimer = Stopwatch.StartNew();
        }

        public bool IsMinimalPackageDelayPassed()
        {
            return !_isNetworkConservationEnabled || _packageTimer.Elapsed > _packedDelay;
        }

        public DatagramPayload CreateHandshakeDatagramPayload()
        {
            return _lastDataSet == null ? CreateHearthBeatDatagramPayload() : CreatePayload(_lastDataSet, DatagramPayloadKind.SessionStart);
        }

        public DatagramPayload CreateHearthBeatDatagramPayload()
        {
            SimulatorDataSet dataSet = new SimulatorDataSet("Remote")
            {
                InputInfo = { BrakePedalPosition = _random.NextDouble(), ClutchPedalPosition = _random.NextDouble(), ThrottlePedalPosition = _random.NextDouble() },
                SessionInfo = new SessionInfo(),
                DriversInfo = new DriverInfo[0],
                PlayerInfo = new DriverInfo(),
                LeaderInfo = new DriverInfo(),
                SimulatorSourceInfo = new SimulatorSourceInfo(),

            };
            return CreatePayload(dataSet, DatagramPayloadKind.HearthBeat);
        }

        public DatagramPayload CreateRegularDatagramPayload(SimulatorDataSet simulatorData)
        {
            _lastDataSet = simulatorData;
            DatagramPayload datagramPayload = CreatePayload(simulatorData, DatagramPayloadKind.Normal);
            RemoveUnnecessaryData(datagramPayload);
            return datagramPayload;
        }

        public DatagramPayload CreateSessionStartedPayload(SimulatorDataSet simulatorData)
        {
            _lastDataSet = simulatorData;
            DatagramPayload datagramPayload = CreatePayload(simulatorData, DatagramPayloadKind.SessionStart);
            RemoveUnnecessaryData(datagramPayload);
            return datagramPayload;
        }

        private DatagramPayload CreatePayload(SimulatorDataSet simulatorDataSet, DatagramPayloadKind payloadKind)
        {
            return new DatagramPayload()
            {
                ContainsOtherDriversTiming = true,
                ContainsPlayersTiming = true,
                ContainsSimulatorSourceInfo = true,
                DriversInfo = simulatorDataSet.DriversInfo,
                InputInfo = simulatorDataSet.InputInfo,
                LeaderInfo = simulatorDataSet.LeaderInfo,
                PayloadKind = payloadKind,
                PlayerInfo = simulatorDataSet.PlayerInfo,
                SessionInfo = simulatorDataSet.SessionInfo,
                Source = simulatorDataSet.Source,
            };
        }

        private void RemoveUnnecessaryData(DatagramPayload datagramPayload)
        {
            if (!_isNetworkConservationEnabled)
            {
                return;
            }

            if (_packageTimer.Elapsed > _packedDelay)
            {
                _packageTimer.Restart();
            }

            if (_playerInfoDelayTimer.Elapsed > _playerInfoDelay)
            {
                _playerInfoDelayTimer.Restart();
            }
            else
            {
                datagramPayload.PlayerInfo = null;
                datagramPayload.ContainsPlayersTiming = false;
                datagramPayload.PlayerInfo = null;
            }

            if (_driversInfoDelayTimer.Elapsed > _driversInfoDelay)
            {
                _driversInfoDelayTimer.Restart();
            }
            else
            {
                datagramPayload.ContainsOtherDriversTiming = false;
                datagramPayload.DriversInfo = null;
                datagramPayload.LeaderInfo = null;
            }

            if (_lastSimulatorSourceName != datagramPayload.Source)
            {
                _lastSimulatorSourceName = datagramPayload.Source;
            }
            else
            {
                datagramPayload.ContainsSimulatorSourceInfo = false;
                datagramPayload.SimulatorSourceInfo = null;
            }
        }
    }
}