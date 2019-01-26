namespace SecondMonitor.Timing.SessionTiming.Drivers.ViewModel
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;
    using DataModel.Telemetry;

    public class LapTelemetryInfo
    {
        private readonly bool _captureDetailedTelemetry;

        public LapTelemetryInfo(DriverInfo driverInfo, SimulatorDataSet dataSet, LapInfo lapInfo, bool captureDetailedTelemetry, TimeSpan snapshotInterval, SimulatorSourceInfo simulatorSourceInfo)
        {
            _captureDetailedTelemetry = captureDetailedTelemetry;
            LapStarSnapshot = new TelemetrySnapshot(driverInfo, dataSet.SessionInfo.WeatherInfo, dataSet.InputInfo, simulatorSourceInfo);
            LapInfo = lapInfo;
            PortionTimes = new LapPortionTimes(10, dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters, lapInfo);
            TimedTelemetrySnapshots = new TimedTelemetrySnapshots(snapshotInterval);
        }

        public TelemetrySnapshot LapEndSnapshot { get; private set; }
        public TelemetrySnapshot LapStarSnapshot { get; }
        public TimedTelemetrySnapshots TimedTelemetrySnapshots { get; private set; }
        public bool IsPurged { get; private set; }
        public LapPortionTimes PortionTimes { get; private set; }
        public LapInfo LapInfo { get; }

        public void CreateLapEndSnapshot(DriverInfo driverInfo, WeatherInfo weather, InputInfo inputInfo, SimulatorSourceInfo simulatorSourceInfo)
        {
            LapEndSnapshot = new TelemetrySnapshot(driverInfo, weather, inputInfo, simulatorSourceInfo);
        }

        public void UpdateTelemetry(SimulatorDataSet dataSet)
        {
            if (IsPurged)
            {
                throw new InvalidOperationException("Cannot update Telemetry on a purged TelemetryInfo");
            }

            PortionTimes.UpdateLapPortions();
            if (_captureDetailedTelemetry)
            {
                TimedTelemetrySnapshots.AddNextSnapshot(LapInfo.CurrentlyValidProgressTime, dataSet.PlayerInfo, dataSet.SessionInfo.WeatherInfo, dataSet.InputInfo, dataSet.SimulatorSourceInfo);
            }
        }

        public void Complete(Distance lapDistance)
        {
            TimedTelemetrySnapshots.TrimInvalid(lapDistance);
        }

        public void Purge()
        {
            if (IsPurged)
            {
                return;
            }

            PortionTimes = null;
            TimedTelemetrySnapshots = null;
            IsPurged = true;
        }
    }
}