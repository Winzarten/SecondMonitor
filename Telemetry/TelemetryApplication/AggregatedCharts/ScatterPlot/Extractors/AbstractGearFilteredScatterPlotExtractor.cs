namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Extractors
{
    using System.Collections.Generic;
    using System.Linq;
    using Filter;
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

        private readonly IReadOnlyCollection<ITelemetryFilter> _allFilters;
        private readonly IGearTelemetryFilter _gearTelemetryFilter;

        protected AbstractGearFilteredScatterPlotExtractor(ISettingsProvider settingsProvider, IEnumerable<ITelemetryFilter> filters, IGearTelemetryFilter gearTelemetryFilter) : base(settingsProvider)
        {
            _allFilters = filters.Append(gearTelemetryFilter).ToList();
            _gearTelemetryFilter = gearTelemetryFilter;
        }

        public ScatterPlotSeries ExtractSeriesForGear(IEnumerable<LapTelemetryDto> loadedLaps, string gear)
        {
            if (!ColorMap.TryGetValue(gear, out OxyColor color))
            {
                color = OxyColors.Azure;
            }

            _gearTelemetryFilter.FilterGear = gear;

            return ExtractSeries(loadedLaps, _allFilters, $"Gear {gear}", color);
        }
    }
}