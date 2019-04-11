namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;
    using Settings;
    using ViewModels.LoadedLapCache;

    public class SuspensionVelocityHistogramDataExtractor : AbstractWheelHistogramDataExtractor
    {
        public SuspensionVelocityHistogramDataExtractor(ISettingsProvider settingsProvider, ILoadedLapsCache loadedLapsCache) : base(settingsProvider, loadedLapsCache)
        {
        }

        protected override bool ZeroBandInMiddle => true;
        protected override double DefaultBandSize => Velocity.FromMs(0.005).GetValueInUnits(VelocityUnitsSmall);
        protected override string Unit => Velocity.GetUnitSymbol(VelocityUnitsSmall);
        protected override Func<WheelInfo, double> WheelValueExtractor => (x) => x.SuspensionVelocity?.GetValueInUnits(VelocityUnitsSmall) ?? 0;
    }
}