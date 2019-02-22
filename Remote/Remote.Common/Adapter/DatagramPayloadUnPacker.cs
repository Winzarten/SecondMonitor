namespace SecondMonitor.Remote.Common.Adapter
{
    using System;
    using System.Linq;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;
    using Model;

    public class DatagramPayloadUnPacker : IDatagramPayloadUnPacker
    {
        private DriverInfo _lastLeaderInfo;
        private DriverInfo _lastPlayerInfo;
        private DriverInfo[] _lastDriversInfo;
        private SimulatorSourceInfo _lastSimulatorSourceInfo;

        public DatagramPayloadUnPacker()
        {
            _lastDriversInfo = new DriverInfo[0];
            _lastPlayerInfo = new DriverInfo();
            _lastLeaderInfo = new DriverInfo();
            _lastSimulatorSourceInfo = new SimulatorSourceInfo();
        }

        public SimulatorDataSet UnpackDatagramPayload(DatagramPayload datagramPayload)
        {
            ExtractLastData(datagramPayload);
            return CreateSimulatorDataSet(datagramPayload);
        }

        private SimulatorDataSet CreateSimulatorDataSet(DatagramPayload datagramPayload)
        {
            SimulatorDataSet newSet = new SimulatorDataSet(datagramPayload.Source)
            {
                DriversInfo = datagramPayload.DriversInfo,
                InputInfo = datagramPayload.InputInfo,
                LeaderInfo = datagramPayload.LeaderInfo,
                PlayerInfo = datagramPayload.PlayerInfo,
                SessionInfo = datagramPayload.SessionInfo,
                SimulatorSourceInfo = datagramPayload.SimulatorSourceInfo,
            };
            FillMissingInformation(newSet);
            return newSet;
        }

        private void FillMissingInformation(SimulatorDataSet simulatorDataSet)
        {
            if (simulatorDataSet.SimulatorSourceInfo == null)
            {
                simulatorDataSet.SimulatorSourceInfo = _lastSimulatorSourceInfo;
            }

            if (simulatorDataSet.PlayerInfo == null)
            {
                simulatorDataSet.PlayerInfo = _lastPlayerInfo;
            }

            if (simulatorDataSet.LeaderInfo == null)
            {
                simulatorDataSet.LeaderInfo = _lastLeaderInfo;
            }

            if (simulatorDataSet.DriversInfo == null)
            {
                simulatorDataSet.DriversInfo = _lastDriversInfo;
            }
        }

        private void ExtractLastData(DatagramPayload datagramPayload)
        {
            if (datagramPayload.ContainsOtherDriversTiming)
            {
                _lastDriversInfo = datagramPayload.DriversInfo;
                _lastLeaderInfo = datagramPayload.LeaderInfo;
                _lastPlayerInfo = datagramPayload.DriversInfo.Any(x => x.IsPlayer) ? datagramPayload.DriversInfo.FirstOrDefault(x => x.IsPlayer) : _lastPlayerInfo;
            }

            if (datagramPayload.ContainsPlayersTiming)
            {
                _lastPlayerInfo = datagramPayload.PlayerInfo;
                if (_lastDriversInfo != null)
                {
                    int index = Array.IndexOf(_lastDriversInfo, _lastDriversInfo.FirstOrDefault(x => x.IsPlayer));
                    if (index != -1)
                    {
                        _lastDriversInfo[index] = _lastPlayerInfo;
                    }
                }
            }

            if (datagramPayload.ContainsSimulatorSourceInfo)
            {
                _lastSimulatorSourceInfo = datagramPayload.SimulatorSourceInfo;
            }
        }
    }
}