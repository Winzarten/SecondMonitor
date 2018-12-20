namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.TelemetryLoad
{
    using System.Threading.Tasks;
    using Repository;
    using Settings;
    using TelemetryManagement.DTO;
    using TelemetryManagement.Repository;

    public class TelemetryLoadController : ITelemetryLoadController
    {
        private readonly ITelemetryRepository _telemetryRepository;

        public TelemetryLoadController(ITelemetryRepositoryFactory telemetryRepositoryFactory, ISettingsProvider settingsProvider )
        {
            _telemetryRepository = telemetryRepositoryFactory.Create(settingsProvider);
        }

        public async Task<SessionInfoDto> LoadSessionAsync(string sessionIdentifier)
        {
            return await Task.Run(() => _telemetryRepository.LoadSessionInformation(sessionIdentifier));
        }
    }
}