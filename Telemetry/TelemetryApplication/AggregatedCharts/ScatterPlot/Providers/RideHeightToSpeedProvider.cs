namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Providers
{
    using Extractors;
    using ViewModels.LoadedLapCache;

    public class RideHeightToSpeedProvider : AbstractWheelChartProvider
    {
        public RideHeightToSpeedProvider(SpeedToRideHeightExtractor dataExtractor, ILoadedLapsCache loadedLaps) : base(dataExtractor, loadedLaps)
        {
        }

        public override string ChartName => "Ride Height / Speed";
    }
}