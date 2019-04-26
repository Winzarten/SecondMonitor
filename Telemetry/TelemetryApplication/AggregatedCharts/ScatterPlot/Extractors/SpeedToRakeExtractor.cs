namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Extractors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;
    using DataModel.Telemetry;
    using Filter;
    using OxyPlot;
    using SecondMonitor.ViewModels.Settings;
    using Settings;
    using TelemetryManagement.DTO;

    public class SpeedToRakeExtractor : AbstractScatterPlotExtractor
    {
        private readonly List<ITelemetryFilter> _filters;

        public SpeedToRakeExtractor(ISettingsProvider settingsProvider, IEnumerable<ITelemetryFilter> filters) : base(settingsProvider)
        {
            _filters = filters.ToList();
        }

        public override string YUnit => Distance.GetUnitsSymbol(DistanceUnitsSmall);
        public override string XUnit => Velocity.GetUnitSymbol(VelocityUnits);
        public override double XMajorTickSize => VelocityUnits == VelocityUnits.Mph ? Velocity.FromMph(50).GetValueInUnits(VelocityUnits) : Velocity.FromKph(50).GetValueInUnits(VelocityUnits);
        public override double YMajorTickSize => Math.Round(Distance.FromMeters(0.05).GetByUnit(DistanceUnitsSmall));

        public ScatterPlotSeries ExtractSeries(IEnumerable<LapTelemetryDto> loadedLaps, string seriesTitle, OxyColor color)
        {
            return ExtractSeries(loadedLaps, _filters, seriesTitle, color);
        }

        protected override double GetXValue(TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.Speed.GetValueInUnits(VelocityUnits);
        }

        protected override double GetYValue(TimedTelemetrySnapshot snapshot)
        {
            double frontHeight = 0;
            double rearHeight = 0;
            CarInfo carInfo = snapshot.PlayerData.CarInfo;
            if (carInfo.FrontHeight != null && carInfo.RearHeight != null && !carInfo.FrontHeight.IsZero)
            {
                rearHeight = carInfo.RearHeight.GetByUnit(DistanceUnitsSmall);
                frontHeight = carInfo.FrontHeight.GetByUnit(DistanceUnitsSmall);
            }
            else if (carInfo.WheelsInfo?.FrontLeft?.RideHeight != null)
            {
                Wheels wheels = carInfo.WheelsInfo;
                frontHeight = (wheels.FrontLeft.RideHeight.GetByUnit(DistanceUnitsSmall) + wheels.FrontRight.RideHeight.GetByUnit(DistanceUnitsSmall)) / 2;
                rearHeight = (wheels.RearLeft.RideHeight.GetByUnit(DistanceUnitsSmall) + wheels.RearRight.RideHeight.GetByUnit(DistanceUnitsSmall)) / 2;
            }

            double rake = rearHeight - frontHeight;
            return rake;
        }
    }
}