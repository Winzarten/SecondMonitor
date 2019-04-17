namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram.Extractors
{
    using System.Collections.Generic;
    using System.Linq;
    using Filter;
    using Settings;
    using TelemetryManagement.DTO;

    public class RpmHistogramDataExtractor : AbstractHistogramDataExtractor
    {
        private readonly IGearTelemetryFilter _gearTelemetryFilter;
        private readonly IReadOnlyCollection<ITelemetryFilter> _allFilters;

        public RpmHistogramDataExtractor(ISettingsProvider settingsProvider, IEnumerable<ITelemetryFilter> telemetryFilters, IGearTelemetryFilter gearTelemetryFilter) : base(settingsProvider)
        {
            _gearTelemetryFilter = gearTelemetryFilter;
            _allFilters = telemetryFilters.Append(gearTelemetryFilter).ToList();
        }

        protected override bool ZeroBandInMiddle => true;

        public override string YUnit => "Rpm";

        public override double DefaultBandSize => 100;

        public Histogram ExtractSeriesForAllGear(IEnumerable<LapTelemetryDto> loadedLaps, double bandSize)
        {
            return ExtractHistogram(loadedLaps, (x) => x.PlayerData.CarInfo.EngineRpm, bandSize, "All Gears");
        }

        public Histogram ExtractSeriesForGear(IEnumerable<LapTelemetryDto> loadedLaps, double bandSize, string gear)
        {
            _gearTelemetryFilter.FilterGear = gear;
            return ExtractHistogram(loadedLaps, (x) => x.PlayerData.CarInfo.EngineRpm, _allFilters, bandSize, $"Gear {gear}");
        }
    }
}