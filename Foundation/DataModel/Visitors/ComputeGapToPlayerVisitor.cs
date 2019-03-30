namespace SecondMonitor.DataModel.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using BasicProperties;
    using Snapshot;
    using Snapshot.Drivers;

    public class ComputeGapToPlayerVisitor : ISimulatorDateSetVisitor
    {
        private readonly TimeSpan _informationValiditySpan;
        private readonly Stopwatch _updateStopwatch;
        private Dictionary<string, TimeSpan> _computedGapToPlayer;

        public ComputeGapToPlayerVisitor(TimeSpan informationValiditySpan)
        {
            _updateStopwatch = new Stopwatch();
            _informationValiditySpan = informationValiditySpan;
            Reset();
        }

        public void Reset()
        {
            _updateStopwatch.Restart();
            _computedGapToPlayer = new Dictionary<string, TimeSpan>();
        }

        public void Visit(SimulatorDataSet simulatorDataSet)
        {
            if (simulatorDataSet.SessionInfo.SessionType != SessionType.Race || simulatorDataSet.SimulatorSourceInfo.GapInformationProvided != GapInformationKind.TimeToSurroundingDrivers)
            {
                return;
            }

            if (_updateStopwatch.Elapsed > _informationValiditySpan)
            {
                ComputeGapToPlayer(simulatorDataSet);
                _updateStopwatch.Restart();
            }

            ApplyGapToPlayer(simulatorDataSet);
            simulatorDataSet.SimulatorSourceInfo.GapInformationProvided = GapInformationKind.Both;
        }

        private void ApplyGapToPlayer(SimulatorDataSet simulatorDataSet)
        {
            foreach (DriverInfo driverInfo in simulatorDataSet.DriversInfo)
            {
                if (driverInfo.IsPlayer)
                {
                    continue;
                }

                if (_computedGapToPlayer.TryGetValue(driverInfo.DriverName, out TimeSpan gap))
                {
                    driverInfo.Timing.GapToPlayer = gap;
                }
            }
        }

        private void ComputeGapToPlayer(SimulatorDataSet simulatorDataSet)
        {
            if (simulatorDataSet.DriversInfo.Length < 2)
            {
                return;
            }
            DriverInfo[] drivers = simulatorDataSet.DriversInfo.OrderBy(x => x.Position).ToArray();
            if (simulatorDataSet.PlayerInfo == null)
            {
                return;
            }

            TimeSpan computedGap = simulatorDataSet.PlayerInfo.Timing.GapAhead;
            for (int i = simulatorDataSet.PlayerInfo.Position -2 ; i >= 0; i--)
            {
                _computedGapToPlayer[drivers[i].DriverName] = computedGap;
                computedGap += drivers[i].Timing.GapAhead;
            }

            computedGap = simulatorDataSet.PlayerInfo.Timing.GapBehind;
            for (int i = simulatorDataSet.PlayerInfo.Position ; i < drivers.Length ; i++)
            {
                _computedGapToPlayer[drivers[i].DriverName] = -computedGap;
                computedGap += drivers[i].Timing.GapBehind;
            }
        }
    }
}