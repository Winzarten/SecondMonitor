namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts
{
    using Histogram;
    using SecondMonitor.ViewModels.Factory;
    using ViewModels.GraphPanel;
    using ViewModels.GraphPanel.Histogram;
    using ViewModels.LoadedLapCache;

    public class SuspensionVelocityHistogramProvider : AbstractWheelHistogramProvider
    {
        public SuspensionVelocityHistogramProvider(SuspensionVelocityHistogramDataExtractor suspensionVelocityHistogramDataExtractor, ILoadedLapsCache loadedLapsCache, IViewModelFactory viewModelFactory) : base(suspensionVelocityHistogramDataExtractor, loadedLapsCache, viewModelFactory)
        {
        }

        public override AggregatedChartViewModel CreateAggregatedChartViewModel()
        {
            return CreateAggregatedChartViewModel<SuspensionVelocityHistogramViewModel, SuspensionVelocityHistogramChartViewModel>();
        }

        public override string ChartName => "Suspension Velocity Histogram";

        public override AggregatedChartKind Kind => AggregatedChartKind.Histogram;
    }
}