namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts
{
    using Histogram;
    using ViewModels.LoadedLapCache;

    public class SuspensionVelocityHistogramProvider : AbstractWheelHistogramProvider
    {

        public SuspensionVelocityHistogramProvider(SuspensionVelocityHistogramDataExtractor suspensionVelocityHistogramDataExtractor, ILoadedLapsCache loadedLapsCache) : base(suspensionVelocityHistogramDataExtractor, loadedLapsCache)
        {
        }

        public override string ChartName => "Suspension Velocity Histogram";
    }
}