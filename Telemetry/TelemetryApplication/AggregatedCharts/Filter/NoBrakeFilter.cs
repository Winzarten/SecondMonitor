namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Filter
{
    using DataModel.Telemetry;

    public class NoBrakeFilter : ITelemetryFilter
    {
        public bool Accepts(TimedTelemetrySnapshot dataSet)
        {
            return dataSet.InputInfo.BrakePedalPosition < 0.01;
        }
    }
}