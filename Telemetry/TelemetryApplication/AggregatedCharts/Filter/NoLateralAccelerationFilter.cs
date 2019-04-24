namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Filter
{
    using System;
    using DataModel.Telemetry;

    public class NoLateralAccelerationFilter : ITelemetryFilter
    {
        public bool Accepts(TimedTelemetrySnapshot dataSet)
        {
            return Math.Abs(dataSet.PlayerData.CarInfo.Acceleration.XinG) < 0.1;
        }
    }
}