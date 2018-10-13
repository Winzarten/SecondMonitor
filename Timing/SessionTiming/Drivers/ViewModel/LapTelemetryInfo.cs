namespace SecondMonitor.Timing.SessionTiming.Drivers.ViewModel
{
    using System;

    using DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;
    using DataModel.Telemetry;

    public class LapTelemetryInfo
    {
        public LapTelemetryInfo(DriverInfo driverInfo, SimulatorDataSet dataSet, LapInfo lapInfo)
        {
            LapStarSnapshot = new TelemetrySnapshot(driverInfo, dataSet.SessionInfo.WeatherInfo);
            LapInfo = lapInfo;
            PortionTimes = new LapPortionTimes(10, dataSet.SessionInfo.TrackInfo.LayoutLength, lapInfo);
        }

        public TelemetrySnapshot LapEndSnapshot { get; private set; }
        public TelemetrySnapshot LapStarSnapshot { get; private set; }
        public bool IsPurged { get; private set; }
        public LapPortionTimes PortionTimes { get; private set; }
        public LapInfo LapInfo { get; }

        public void CreateLapEndSnapshot(DriverInfo driverInfo, WeatherInfo weather)
        {
            LapEndSnapshot = new TelemetrySnapshot(driverInfo, weather);
        }

        public void UpdateTelemetry()
        {
            if (IsPurged)
            {
                throw new InvalidOperationException("Cannot update Telemetry on a purged TelemetryInfo");
            }

            PortionTimes.UpdateLapPortions();
        }

        public void Purge()
        {
            if (IsPurged)
            {
                return;
            }

            LapEndSnapshot = null;
            LapStarSnapshot = null;
            PortionTimes = null;
            IsPurged = true;
        }
    }
}