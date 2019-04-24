namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Providers
{
    using Extractors;
    using ViewModels.LoadedLapCache;

    public class RideHeightToHorizontalAccProvider : AbstractWheelChartProvider
    {
        public RideHeightToHorizontalAccProvider(HorizontalAccelerationToRideHeightExtractor dataExtractor, ILoadedLapsCache loadedLaps) : base(dataExtractor, loadedLaps)
        {
        }

        public override string ChartName => "Ride Height / Horizontal Acceleration";
    }
}