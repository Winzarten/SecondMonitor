namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Filter
{
    using System;
    using DataModel.Telemetry;

    public class LateralAccFilter : ITelemetryFilter
    {
        public double MinimumG { get; set; }
        public double MaximumG { get; set; }

        public bool Accepts(TimedTelemetrySnapshot dataSet)
        {
            double g = Math.Abs(dataSet.PlayerData.CarInfo.Acceleration.XinG);
            return g >= MinimumG && g < MaximumG;
        }
    }
}