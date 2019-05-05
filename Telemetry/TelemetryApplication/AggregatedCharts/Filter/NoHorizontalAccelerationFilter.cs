namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Filter
{
    using System;
    using DataModel.Telemetry;

    public class NoHorizontalAccelerationFilter : ITelemetryFilter
    {
        public bool Accepts(TimedTelemetrySnapshot dataSet)
        {
            return Math.Abs(dataSet.PlayerData.CarInfo.Acceleration.ZinG) < 0.2;
        }
    }
}