using System;
using System.Collections.Generic;
using SecondMonitor.DataModel.Snapshot;
using SecondMonitor.DataModel.Snapshot.Drivers;

namespace SecondMonitor.DataModel.Telemetry
{
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

        public void AddNextSnapshot(TimeSpan lapTime, DriverInfo playerInfo, WeatherInfo weatherInfo)
        {
            if ((playerInfo.InPits && playerInfo.Speed.InKph < 5) || lapTime < _nextSnapshotTime)
            {
                return;
            }

            _nextSnapshotTime = lapTime + _snapshotsIntervals;
            _snapshots.Add(new TimedTelemetrySnapshot(lapTime, playerInfo, weatherInfo));
        }
    }
}