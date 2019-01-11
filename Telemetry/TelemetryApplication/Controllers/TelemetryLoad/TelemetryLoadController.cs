namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.TelemetryLoad
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NLog;
    using Repository;
    using Settings;
    using Synchronization;
    using TelemetryManagement.DTO;
    using TelemetryManagement.Repository;

    public class TelemetryLoadController : ITelemetryLoadController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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

        public async Task<IReadOnlyCollection<SessionInfoDto>> GetAllRecentSessionInfoAsync()
        {
            try
            {
                return await Task.Run(() => _telemetryRepository.GetAllRecentSessions());
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while loading recent sessions");
            }

            return Enumerable.Empty<SessionInfoDto>().ToList().AsReadOnly();
        }

        public async Task<SessionInfoDto> LoadRecentSessionAsync(string sessionIdentifier)
        {
            try
            {
                SessionInfoDto sessionInfoDto = await Task.Run(() => _telemetryRepository.LoadRecentSessionInformation(sessionIdentifier));
                return await LoadRecentSessionAsync(sessionInfoDto);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while loading session");
            }

            return null;
        }

        public async Task<SessionInfoDto> LoadRecentSessionAsync(SessionInfoDto sessionInfoDto)
        {
            await CloseCurrentSession();
            _cachedTelemetries.Clear();
            _telemetryViewsSynchronization.NotifyNewSessionLoaded(sessionInfoDto);
            LastLoadedSessionIdentifier = sessionInfoDto.Id;
            return sessionInfoDto;
        }

        public async Task<SessionInfoDto> LoadLastSessionAsync()
        {
            try
            {
                string sessionIdent = _telemetryRepository.GetLastRecentSessionIdentifier();
                if (string.IsNullOrEmpty(sessionIdent))
                {
                    return null;
                }

                return await LoadRecentSessionAsync(sessionIdent);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while loading last session");
            }

            return null;
        }


        public async Task<LapTelemetryDto> LoadLap(int lapNumber)
        {
            try
            {
                AddToActiveLapJob();
                if (!_cachedTelemetries.TryGetValue(lapNumber, out LapTelemetryDto lapTelemetryDto))
                {
                    lapTelemetryDto = await Task.Run(() => _telemetryRepository.LoadRecentLapTelemetryDto(LastLoadedSessionIdentifier, lapNumber));
                    _cachedTelemetries[lapNumber] = lapTelemetryDto;
                }

                _telemetryViewsSynchronization.NotifyLapLoaded(lapTelemetryDto);
                RemoveFromActiveLapJob();
                return lapTelemetryDto;
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"Error while loading lap telemetry");
                return null;
            }
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

        private async Task CloseCurrentSession()
        {
            while (_activeLapJobs > 0)
            {
                await Task.Delay(100);
            }
            foreach (LapTelemetryDto value in _cachedTelemetries.Values)
            {
                await UnloadLap(value.LapSummary);
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