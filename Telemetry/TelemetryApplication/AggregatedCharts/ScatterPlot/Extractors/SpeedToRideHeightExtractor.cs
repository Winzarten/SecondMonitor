namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Extractors
{
    using System;
    using System.Collections.Generic;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;
    using DataModel.Telemetry;
    using Filter;
    using SecondMonitor.ViewModels.Settings;
    using Settings;

    public class SpeedToRideHeightExtractor : AbstractWheelScatterPlotDataExtractor
    {
        public SpeedToRideHeightExtractor(ISettingsProvider settingsProvider, IEnumerable<ITelemetryFilter> filters) : base(settingsProvider, filters)
        {
        }

        public override string YUnit => Distance.GetUnitsSymbol(DistanceUnitsSmall);

        public override string XUnit => Velocity.GetUnitSymbol(VelocityUnits);

        public override double XMajorTickSize => VelocityUnits == VelocityUnits.Mph ? Velocity.FromMph(50).GetValueInUnits(VelocityUnits) : Velocity.FromKph(50).GetValueInUnits(VelocityUnits);
        public override double YMajorTickSize => Math.Round(Distance.FromMeters(0.05).GetByUnit(DistanceUnitsSmall));

        protected override double GetXWheelValue(WheelInfo wheelInfo, TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.Speed.GetValueInUnits(VelocityUnits);
        }

        protected override double GetYWheelValue(WheelInfo wheelInfo, TimedTelemetrySnapshot snapshot)
        {
            return wheelInfo.RideHeight.GetByUnit(DistanceUnitsSmall);
        }
    }
}