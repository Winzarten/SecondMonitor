using System;
using System.Collections.Generic;
using SecondMonitor.DataModel.Snapshot;
using SecondMonitor.DataModel.Snapshot.Drivers;

namespace SecondMonitor.DataModel.Telemetry
{
    using BasicProperties;

    public class TimedTelemetrySnapshots
    {
        private readonly TimeSpan _snapshotsIntervals;
        private readonly List<TimedTelemetrySnapshot> _snapshots;
        private TimeSpan _nextSnapshotTime;

        public TimedTelemetrySnapshots(TimeSpan snapshotsIntervals)
        {
            _snapshotsIntervals = snapshotsIntervals == TimeSpan.Zero ? TimeSpan.FromMilliseconds(5) : snapshotsIntervals ;
            _nextSnapshotTime = TimeSpan.Zero;;
            _snapshots = new List<TimedTelemetrySnapshot>(120000 / (int)_snapshotsIntervals.TotalMilliseconds);
        }

        public IReadOnlyCollection<TimedTelemetrySnapshot> Snapshots => _snapshots.AsReadOnly();

        public void AddNextSnapshot(TimeSpan lapTime, DriverInfo playerInfo, WeatherInfo weatherInfo, InputInfo inputInfo, SimulatorSourceInfo simulatorSource)
        {
            if ((playerInfo.InPits && playerInfo.Speed.InKph < 5) || lapTime < _nextSnapshotTime)
            {
                return;
            }

            _nextSnapshotTime = lapTime + _snapshotsIntervals;
            _snapshots.Add(new TimedTelemetrySnapshot(lapTime, playerInfo, weatherInfo, inputInfo, simulatorSource));
        }

        public void TrimInvalid(Distance lapDistance)
        {
            if (_snapshots.Count == 0)
            {
                return;
            }

            bool start = true;
            bool end = false;
            double distanceThresholdUp = lapDistance.InMeters * 0.75;
            double distanceThresholdDown = lapDistance.InMeters * 0.25;
            List<TimedTelemetrySnapshot> toRemove = new List<TimedTelemetrySnapshot>(2);

            foreach (TimedTelemetrySnapshot currentSnapshot in _snapshots)
            {
                if (start && currentSnapshot.PlayerData.LapDistance >= distanceThresholdUp)
                {
                    toRemove.Add(currentSnapshot);
                    continue;
                }

                if (start)
                {
                    start = false;
                }

                if (!end && currentSnapshot.PlayerData.LapDistance >= distanceThresholdUp)
                {
                    end = true;
                    continue;
                }

                if(end && currentSnapshot.PlayerData.LapDistance <= distanceThresholdDown)
                {
                    toRemove.Add(currentSnapshot);
                }
            }

            toRemove.ForEach(x=> _snapshots.Remove(x));
        }
    }
}