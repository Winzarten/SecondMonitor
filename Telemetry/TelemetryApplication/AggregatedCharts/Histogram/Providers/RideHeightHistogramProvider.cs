namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram.Providers
{
    using Extractors;
    using SecondMonitor.ViewModels.Factory;
    using ViewModels.LoadedLapCache;

    public class RideHeightHistogramProvider : AbstractWheelHistogramProvider
    {
        public RideHeightHistogramProvider(RideHeightHistogramExtractor abstractWheelHistogramDataExtractor, ILoadedLapsCache loadedLapsCache, IViewModelFactory viewModelFactory) : base(abstractWheelHistogramDataExtractor, loadedLapsCache, viewModelFactory)
        {
        }

        public override string ChartName => "Ride Height (Wheels)";

        public override AggregatedChartKind Kind => AggregatedChartKind.Histogram;
    }
}