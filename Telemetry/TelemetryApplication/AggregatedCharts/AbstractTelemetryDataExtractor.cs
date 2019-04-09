namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;
    using Settings;
    using ViewModels.LoadedLapCache;

    public abstract class AbstractTelemetryDataExtractor
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly ILoadedLapsCache _loadedLapsCache;

        protected AbstractTelemetryDataExtractor(ISettingsProvider settingsProvider, ILoadedLapsCache loadedLapsCache)
        {
            _settingsProvider = settingsProvider;
            _loadedLapsCache = loadedLapsCache;

            VelocityUnitsSmall = settingsProvider.DisplaySettingsViewModel.VelocityUnitsVerySmall;
        }

        protected VelocityUnits VelocityUnitsSmall { get; set; }

        public double[] ExtractData(Func<TimedTelemetrySnapshot, double> extractionFunc)
        {
            List<TimedTelemetrySnapshot> timedTelemetrySnapshots = _loadedLapsCache.LoadedLaps.SelectMany(x => x.TimedTelemetrySnapshots).ToList();
            return timedTelemetrySnapshots.Select(extractionFunc).ToArray();
        }
    }
}