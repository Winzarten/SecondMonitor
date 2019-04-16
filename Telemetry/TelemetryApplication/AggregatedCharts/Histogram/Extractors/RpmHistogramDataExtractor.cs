namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram.Extractors
{
    using System.Collections.Generic;
    using Settings;
    using TelemetryManagement.DTO;

    public class RpmHistogramDataExtractor : AbstractHistogramDataExtractor
    {
        public RpmHistogramDataExtractor(ISettingsProvider settingsProvider) : base(settingsProvider)
        {
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
            return ExtractHistogram(loadedLaps, (x) => x.PlayerData.CarInfo.EngineRpm, x => x.PlayerData.CarInfo.CurrentGear == gear, bandSize, $"Gear {gear}");
        }
    }
}