namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Extractors
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;
    using Settings;

    public class SpeedToDownforceExtractor : AbstractScatterPlotExtractor
    {
        public SpeedToDownforceExtractor(ISettingsProvider settingsProvider) : base(settingsProvider)
        {
        }

        public override string YUnit => Force.GetUnitSymbol(ForceUnits);

        public override string XUnit => Velocity.GetUnitSymbol(VelocityUnits);

        public override double XMajorTickSize => VelocityUnits == VelocityUnits.Mph ? Velocity.FromMph(50).GetValueInUnits(VelocityUnits) : Velocity.FromKph(50).GetValueInUnits(VelocityUnits);
        public override double YMajorTickSize => Math.Round(Force.GetFromNewtons(2500).GetValueInUnits(ForceUnits));

        protected override double GetXValue(TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.Speed.GetValueInUnits(VelocityUnits);
        }

        protected override double GetYValue(TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.CarInfo.OverallDownForce?.GetValueInUnits(ForceUnits) ?? 0;
        }
    }
}