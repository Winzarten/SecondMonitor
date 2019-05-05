namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram.Providers
{
    using Extractors;
    using SecondMonitor.ViewModels.Factory;
    using ViewModels.AggregatedCharts;
    using ViewModels.AggregatedCharts.Histogram;
    using ViewModels.LoadedLapCache;

    public class SuspensionVelocityHistogramProvider : AbstractWheelHistogramProvider
    {
        public SuspensionVelocityHistogramProvider(SuspensionVelocityHistogramDataExtractor suspensionVelocityHistogramDataExtractor, ILoadedLapsCache loadedLapsCache, IViewModelFactory viewModelFactory) : base(suspensionVelocityHistogramDataExtractor, loadedLapsCache, viewModelFactory)
        {
        }

        public override IAggregatedChartViewModel CreateAggregatedChartViewModel()
        {
            return CreateAggregatedChartViewModel<WheelsHistogramChartViewModel, SuspensionVelocityHistogramChartViewModel>();
        }

        public override string ChartName => "Suspension Velocity Histogram";

        public override AggregatedChartKind Kind => AggregatedChartKind.Histogram;
    }
}