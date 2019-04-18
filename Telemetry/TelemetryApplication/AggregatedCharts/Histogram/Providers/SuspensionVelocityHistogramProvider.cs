namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram.Providers
{
    using AggregatedCharts.Histogram;
    using Extractors;
    using SecondMonitor.ViewModels.Factory;
    using ViewModels.AggregatedCharts;
    using ViewModels.GraphPanel.Histogram;
    using ViewModels.LoadedLapCache;

    public class SuspensionVelocityHistogramProvider : AbstractWheelHistogramProvider
    {
        public SuspensionVelocityHistogramProvider(SuspensionVelocityHistogramDataExtractor suspensionVelocityHistogramDataExtractor, ILoadedLapsCache loadedLapsCache, IViewModelFactory viewModelFactory) : base(suspensionVelocityHistogramDataExtractor, loadedLapsCache, viewModelFactory)
        {
        }

        public override IAggregatedChartViewModel CreateAggregatedChartViewModel()
        {
            return CreateAggregatedChartViewModel<SuspensionVelocityHistogramViewModel, SuspensionVelocityHistogramChartViewModel>();
        }

        public override string ChartName => "Suspension Velocity Histogram";

        public override AggregatedChartKind Kind => AggregatedChartKind.Histogram;
    }
}