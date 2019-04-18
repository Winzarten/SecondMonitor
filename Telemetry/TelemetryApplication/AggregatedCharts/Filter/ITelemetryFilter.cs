namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Filter
{
    using DataModel.Snapshot;
    using DataModel.Telemetry;

    public interface ITelemetryFilter
    {
        bool Accepts(TimedTelemetrySnapshot dataSet);
    }
}