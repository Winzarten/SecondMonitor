namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Extractors
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;
    using Settings;

    public class SpeedToHorizontalGExtractor : AbstractGearFilteredScatterPlotExtractor
    {
        public SpeedToHorizontalGExtractor(ISettingsProvider settingsProvider) : base(settingsProvider)
        {
        }

        public override string YUnit => "G";

        public override string XUnit => Velocity.GetUnitSymbol(VelocityUnits);

        public override double XMajorTickSize => VelocityUnits == VelocityUnits.Mph ? Velocity.FromMph(50).GetValueInUnits(VelocityUnits) : Velocity.FromKph(50).GetValueInUnits(VelocityUnits);

        public override double YMajorTickSize => 1;

        protected override double GetXValue(TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.Speed.GetValueInUnits(VelocityUnits);
        }

        protected override double GetYValue(TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.CarInfo.Acceleration.ZinG;
        }
    }
}