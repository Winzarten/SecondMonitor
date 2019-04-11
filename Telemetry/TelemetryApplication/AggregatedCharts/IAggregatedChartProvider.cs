namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts
{
    using SecondMonitor.ViewModels;
    using ViewModels.GraphPanel;

    public interface IAggregatedChartProvider
    {
        string ChartName { get; }

        AggregatedChartViewModel CreateAggregatedChartViewModel();
    }
}