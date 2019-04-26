namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram.Extractors
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;
    using SecondMonitor.ViewModels.Settings;
    using Settings;

    public class SuspensionVelocityHistogramDataExtractor : AbstractWheelHistogramDataExtractor
    {
        public SuspensionVelocityHistogramDataExtractor(ISettingsProvider settingsProvider) : base(settingsProvider)
        {
        }

        protected override bool ZeroBandInMiddle => false;
        public override double DefaultBandSize => Math.Round(Velocity.FromMs(0.005).GetValueInUnits(VelocityUnitsSmall), 2);
        public override string YUnit => Velocity.GetUnitSymbol(VelocityUnitsSmall);
        protected override Func<WheelInfo, double> WheelValueExtractor => (x) => x.SuspensionVelocity?.GetValueInUnits(VelocityUnitsSmall) ?? 0;
    }
}