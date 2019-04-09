namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts
{
    using SecondMonitor.ViewModels;

    public class SuspensionSpeedHistogramProvider : IAggregatedChartProvider
    {
        public string ChartName => "Suspension Speed";
        public AbstractViewModel CreateAggregatedChartViewModel()
        {
            throw new System.NotImplementedException();
        }
    }
}