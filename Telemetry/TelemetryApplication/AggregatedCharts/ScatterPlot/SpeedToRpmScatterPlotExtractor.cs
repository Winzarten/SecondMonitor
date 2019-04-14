namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot
{
    using System.Collections.Generic;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;
    using OxyPlot;
    using Settings;
    using TelemetryManagement.DTO;

    public class SpeedToRpmScatterPlotExtractor : AbstractScatterPlotExtractor
    {
        private static readonly Dictionary<string, OxyColor> ColorMap = new Dictionary<string, OxyColor>()
        {
            {"N", OxyColor.Parse("#ff9900")},
            {"1", OxyColor.Parse("#99ffcc")},
            {"2", OxyColor.Parse("#0066ff")},
            {"3", OxyColor.Parse("#33cccc")},
            {"4", OxyColor.Parse("#00cc00")},
            {"5", OxyColor.Parse("#ff531a")},
            {"6", OxyColor.Parse("#1aff1a")},
            {"7", OxyColor.Parse("#0000ff")},
            {"8", OxyColor.Parse("#cccc00")},
        };

        public SpeedToRpmScatterPlotExtractor(ISettingsProvider settingsProvider) : base(settingsProvider)
        {
        }

        public override string YUnit => "RPM";

        public override string XUnit => Velocity.GetUnitSymbol(VelocityUnits);

        public override double XMajorTickSize => VelocityUnits == VelocityUnits.Mph ? Velocity.FromMph(10).GetValueInUnits(VelocityUnits) : Velocity.FromKph(10).GetValueInUnits(VelocityUnits);

        public override double YMajorTickSize => 200;

        protected override double GetXValue(TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.Speed.GetValueInUnits(VelocityUnits);
        }

        protected override double GetYValue(TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.CarInfo.EngineRpm;
        }

        public ScatterPlotSeries ExtractSeriesForGear(IEnumerable<LapTelemetryDto> loadedLaps, string gear)
        {
            if (!ColorMap.TryGetValue(gear, out OxyColor color))
            {
                color = OxyColors.Azure;
            }

            return ExtractSeries(loadedLaps, x => x.InputInfo.ThrottlePedalPosition > 0.9 && x.PlayerData.CarInfo.CurrentGear == gear, $"Gear {gear}", color);
        }
    }
}