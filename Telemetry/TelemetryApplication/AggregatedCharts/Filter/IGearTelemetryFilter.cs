namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Filter
{
    public interface IGearTelemetryFilter : ITelemetryFilter
    {
        string FilterGear { get; set; }
    }
}