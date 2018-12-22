namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.TelemetryLoad
{
    using System.Threading.Tasks;
    using Repository;
    using Settings;
    using Synchronization;
    using TelemetryManagement.DTO;
    using TelemetryManagement.Repository;

    public class TelemetryLoadController : ITelemetryLoadController
    {
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;
        private readonly ITelemetryRepository _telemetryRepository;

        public TelemetryLoadController(ITelemetryRepositoryFactory telemetryRepositoryFactory, ISettingsProvider settingsProvider, ITelemetryViewsSynchronization telemetryViewsSynchronization )
        {
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _telemetryRepository = telemetryRepositoryFactory.Create(settingsProvider);
        }

        public string LastLoadedSessionIdentifier { get; private set; }

        public async Task<SessionInfoDto> LoadSessionAsync(string sessionIdentifier)
        {
            SessionInfoDto sessionInfoDto = await Task.Run(() => _telemetryRepository.LoadSessionInformation(sessionIdentifier));
            _telemetryViewsSynchronization.NotifyNewSessionLoaded(sessionInfoDto);
            LastLoadedSessionIdentifier = sessionIdentifier;
            return sessionInfoDto;
        }

        public async Task<LapTelemetryDto> LoadLap(int lapNumber)
        {
            LapTelemetryDto lapTelemetryDto = await Task.Run(() => _telemetryRepository.LoadLapTelemetryDto(LastLoadedSessionIdentifier, lapNumber));
            _telemetryViewsSynchronization.NotifyLapLoaded(lapTelemetryDto);
            return lapTelemetryDto;
        }

        public Task UnloadLap(LapSummaryDto lapSummaryDto)
        {
            _telemetryViewsSynchronization.NotifyLapUnloaded(lapSummaryDto);
            return Task.CompletedTask;
        }
    }
}