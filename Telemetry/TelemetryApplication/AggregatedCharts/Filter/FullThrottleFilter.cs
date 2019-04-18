namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Filter
{
    using DataModel.Telemetry;

    public class FullThrottleFilter : ITelemetryFilter
    {
        public bool Accepts(TimedTelemetrySnapshot dataSet)
        {
            return dataSet.InputInfo.ThrottlePedalPosition > 0.95;
        }
    }
}