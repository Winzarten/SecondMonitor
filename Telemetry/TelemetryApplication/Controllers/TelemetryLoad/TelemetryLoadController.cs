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

        public async Task<SessionInfoDto> LoadSessionAsync(string sessionIdentifier)
        {
            SessionInfoDto sessionInfoDto = await Task.Run(() => _telemetryRepository.LoadSessionInformation(sessionIdentifier));
            _telemetryViewsSynchronization.NotifyNewSessionLoaded(sessionInfoDto);
            return sessionInfoDto;
        }
    }
}