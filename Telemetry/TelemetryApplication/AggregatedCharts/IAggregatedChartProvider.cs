namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts
{
    using SecondMonitor.ViewModels;

    public interface IAggregatedChartProvider
    {
        string ChartName { get; }

        AbstractViewModel CreateAggregatedChartViewModel();
    }
}