namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Extractors
{
    using System.Collections.Generic;
    using OxyPlot;
    using Settings;
    using TelemetryManagement.DTO;

    public abstract class AbstractGearFilteredScatterPlotExtractor : AbstractScatterPlotExtractor
    {
        protected static readonly Dictionary<string, OxyColor> ColorMap = new Dictionary<string, OxyColor>()
        {
            {"N", OxyColor.Parse("#ff9900")},
            {"1", OxyColor.Parse("#99ffcc")},
            {"2", OxyColor.Parse("#0066ff")},
            {"3", OxyColor.Parse("#33cccc")},
            {"4", OxyColor.Parse("#E5FBE4")},
            {"5", OxyColor.Parse("#ff531a")},
            {"6", OxyColor.Parse("#1aff1a")},
            {"7", OxyColor.Parse("#0000ff")},
            {"8", OxyColor.Parse("#cccc00")},
        };

        protected AbstractGearFilteredScatterPlotExtractor(ISettingsProvider settingsProvider) : base(settingsProvider)
        {
        }

        public ScatterPlotSeries ExtractSeriesForGear(IEnumerable<LapTelemetryDto> loadedLaps, string gear)
        {
            if (!ColorMap.TryGetValue(gear, out OxyColor color))
            {
                color = OxyColors.Azure;
            }

            return ExtractSeries(loadedLaps, x => x.InputInfo.ThrottlePedalPosition > 0.95 && x.PlayerData.CarInfo.CurrentGear == gear, $"Gear {gear}", color);
        }
    }
}