namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts
{
    using ViewModels.GraphPanel;

    public interface IAggregatedChartProvider
    {
        string ChartName { get; }
        AggregatedChartKind Kind { get; }

        AggregatedChartViewModel CreateAggregatedChartViewModel();
    }
}