namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Providers
{
    using Extractors;
    using ViewModels.LoadedLapCache;

    public class RideHeightToLateralAccProvider : AbstractWheelChartProvider
    {
        public RideHeightToLateralAccProvider(LateralAccelerationToRideHeightExtractor dataExtractor, ILoadedLapsCache loadedLaps) : base(dataExtractor, loadedLaps)
        {
        }

        public override string ChartName => "Ride Height / Lateral Acceleration";
    }
}