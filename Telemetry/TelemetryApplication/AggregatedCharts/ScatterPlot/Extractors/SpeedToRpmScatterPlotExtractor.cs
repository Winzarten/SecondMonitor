namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Extractors
{
    using System.Collections.Generic;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;
    using Filter;
    using SecondMonitor.ViewModels.Settings;
    using Settings;

    public class SpeedToRpmScatterPlotExtractor : AbstractGearFilteredScatterPlotExtractor
    {


        public SpeedToRpmScatterPlotExtractor(ISettingsProvider settingsProvider, IEnumerable<ITelemetryFilter> filters, IGearTelemetryFilter gearTelemetryFilter) : base(settingsProvider, filters, gearTelemetryFilter)
        {
        }

        public override string YUnit => "RPM";

        public override string XUnit => Velocity.GetUnitSymbol(VelocityUnits);

        public override double XMajorTickSize => VelocityUnits == VelocityUnits.Mph ? Velocity.FromMph(50).GetValueInUnits(VelocityUnits) : Velocity.FromKph(50).GetValueInUnits(VelocityUnits);

        public override double YMajorTickSize => 1000;

        protected override double GetXValue(TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.Speed.GetValueInUnits(VelocityUnits);
        }

        protected override double GetYValue(TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.CarInfo.EngineRpm;
        }
    }
}