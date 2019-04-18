namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.AggregatedCharts
{
    using SecondMonitor.ViewModels;

    public interface IAggregatedChartViewModel : IViewModel
    {
        string Title { get; }
    }
}