namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.TelemetryLoad
{
    using System.Collections.Generic;
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
        private readonly Dictionary<int, LapTelemetryDto> _cachedTelemetries;
        private int _activeLapJobs;

        public TelemetryLoadController(ITelemetryRepositoryFactory telemetryRepositoryFactory, ISettingsProvider settingsProvider, ITelemetryViewsSynchronization telemetryViewsSynchronization )
        {
            _cachedTelemetries = new Dictionary<int, LapTelemetryDto>();
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _telemetryRepository = telemetryRepositoryFactory.Create(settingsProvider);
        }

        public string LastLoadedSessionIdentifier { get; private set; }

        public async Task<SessionInfoDto> LoadSessionAsync(string sessionIdentifier)
        {
            _cachedTelemetries.Clear();
            SessionInfoDto sessionInfoDto = await Task.Run(() => _telemetryRepository.LoadSessionInformation(sessionIdentifier));
            _telemetryViewsSynchronization.NotifyNewSessionLoaded(sessionInfoDto);
            LastLoadedSessionIdentifier = sessionIdentifier;
            return sessionInfoDto;
        }

        public async Task<SessionInfoDto> LoadLastSessionAsync()
        {
            string sessionIdent = _telemetryRepository.GetLastSessionIdentifier();
            return await LoadSessionAsync(sessionIdent);
        }


        public async Task<LapTelemetryDto> LoadLap(int lapNumber)
        {
            AddToActiveLapJob();
            if (!_cachedTelemetries.TryGetValue(lapNumber, out LapTelemetryDto lapTelemetryDto))
            {
                lapTelemetryDto = await Task.Run(() => _telemetryRepository.LoadLapTelemetryDto(LastLoadedSessionIdentifier, lapNumber));
                _cachedTelemetries[lapNumber] = lapTelemetryDto;
            }
            _telemetryViewsSynchronization.NotifyLapLoaded(lapTelemetryDto);
            RemoveFromActiveLapJob();
            return lapTelemetryDto;
        }

        public Task UnloadLap(LapSummaryDto lapSummaryDto)
        {
            _telemetryViewsSynchronization.NotifyLapUnloaded(lapSummaryDto);
            return Task.CompletedTask;
        }

        private void AddToActiveLapJob()
        {
            _activeLapJobs++;
            if (_activeLapJobs == 1)
            {
                _telemetryViewsSynchronization.NotifyLapLoadingStarted();
            }
        }

        private void RemoveFromActiveLapJob()
        {
            _activeLapJobs--;
            if (_activeLapJobs == 0)
            {
                _telemetryViewsSynchronization.NotifyLapLoadingFinished();
            }
        }
    }
}