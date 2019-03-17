namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LoadedLapCache
{
    using System.Collections.Generic;
    using Controllers.Synchronization;
    using TelemetryManagement.DTO;

    public class LoadedLapsCache : ILoadedLapsCache
    {
        private readonly List<LapTelemetryDto> _loadedLaps;

        public LoadedLapsCache(ITelemetryViewsSynchronization telemetryViewsSynchronization)
        {
            _loadedLaps = new List<LapTelemetryDto>();
            telemetryViewsSynchronization.LapLoaded += TelemetryViewsSynchronizationOnLapLoaded;
            telemetryViewsSynchronization.LapUnloaded += TelemetryViewsSynchronizationOnLapUnloaded;
            telemetryViewsSynchronization.ReferenceLapSelected += TelemetryViewsSynchronizationOnReferenceLapSelected;
        }

        public IReadOnlyCollection<LapTelemetryDto> LoadedLaps => _loadedLaps.AsReadOnly();

        public LapSummaryDto ReferenceLap { get; set; }

        private void TelemetryViewsSynchronizationOnReferenceLapSelected(object sender, LapSummaryArgs e)
        {
            ReferenceLap = e.LapSummary;
        }

        private void TelemetryViewsSynchronizationOnLapUnloaded(object sender, LapSummaryArgs e)
        {
            _loadedLaps.RemoveAll(x => x.LapSummary.Id == e.LapSummary.Id);
        }

        private void TelemetryViewsSynchronizationOnLapLoaded(object sender, LapTelemetryArgs e)
        {
            _loadedLaps.Add(e.LapTelemetry);
        }
    }
}