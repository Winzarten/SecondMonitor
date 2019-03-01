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
        private readonly Dictionary<string, LapTelemetryDto> _cachedTelemetries;
        private int _activeLapJobs;
        private readonly List<string> _loadedSessions;

        public TelemetryLoadController(ITelemetryRepositoryFactory telemetryRepositoryFactory, ISettingsProvider settingsProvider, ITelemetryViewsSynchronization telemetryViewsSynchronization )
        {
            _cachedTelemetries = new Dictionary<string, LapTelemetryDto>();
            _loadedSessions = new List<string>();
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _telemetryRepository = telemetryRepositoryFactory.Create(settingsProvider);
        }

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
                SessionInfoDto sessionInfoDto = await Task.Run(() => _telemetryRepository.OpenRecentSession(sessionIdentifier));
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
            await CloseAllOpenedSessions();
            _cachedTelemetries.Clear();
            _loadedSessions.Add(sessionInfoDto.Id);
            sessionInfoDto.LapsSummary.ForEach(FillCustomDisplayName);
            _telemetryViewsSynchronization.NotifyNewSessionLoaded(sessionInfoDto);
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


        public async Task<LapTelemetryDto> LoadLap(LapSummaryDto lapSummaryDto)
        {
            try
            {
                AddToActiveLapJob();
                if (!_cachedTelemetries.TryGetValue(lapSummaryDto.Id, out LapTelemetryDto lapTelemetryDto))
                {
                    lapTelemetryDto = await Task.Run(() => _telemetryRepository.LoadLapTelemetryDtoFromAnySession(lapSummaryDto));
                    FillCustomDisplayName(lapTelemetryDto.LapSummary);
                    _cachedTelemetries[lapTelemetryDto.LapSummary.Id] = lapTelemetryDto;
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

        private async Task CloseAllOpenedSessions()
        {
            while (_activeLapJobs > 0)
            {
                await Task.Delay(100);
            }
            foreach (LapTelemetryDto value in _cachedTelemetries.Values)
            {
                await UnloadLap(value.LapSummary);
            }
            _loadedSessions.ForEach( x => _telemetryRepository.CloseRecentSession(x));
            _loadedSessions.Clear();
        }

        private void RemoveFromActiveLapJob()
        {
            _activeLapJobs--;
            if (_activeLapJobs == 0)
            {
                _telemetryViewsSynchronization.NotifyLapLoadingFinished();
            }
        }

        private void FillCustomDisplayName(LapSummaryDto lapSummaryDto)
        {
            int sessionIndex = _loadedSessions.IndexOf(lapSummaryDto.SessionIdentifier);
            lapSummaryDto.CustomDisplayName = sessionIndex > 0 ? $"{sessionIndex}/{lapSummaryDto.LapNumber}" : lapSummaryDto.LapNumber.ToString();
        }
    }
}