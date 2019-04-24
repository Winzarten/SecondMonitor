namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram.Extractors
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;
    using Settings;

    public class RideHeightHistogramExtractor : AbstractWheelHistogramDataExtractor
    {
        public RideHeightHistogramExtractor(ISettingsProvider settingsProvider) : base(settingsProvider)
        {
        }

        protected override bool ZeroBandInMiddle => true;
        public override double DefaultBandSize => Math.Round(Distance.FromMeters(0.005).GetByUnit(DistanceUnitsSmall), 2);
        public override string YUnit => Distance.GetUnitsSymbol(DistanceUnitsSmall);
        protected override Func<WheelInfo, double> WheelValueExtractor => (x) => x.RideHeight?.GetByUnit(DistanceUnitsSmall) ?? 0;
    }
}