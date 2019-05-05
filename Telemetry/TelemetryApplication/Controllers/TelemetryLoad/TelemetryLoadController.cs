namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.TelemetryLoad
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
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
        private readonly ConcurrentDictionary<string, LapTelemetryDto> _cachedTelemetries;
        private int _activeLapJobs;
        private readonly List<string> _loadedSessions;
        private readonly List<string> _knownLaps;
        private Task _loopTask;
        private CancellationTokenSource _loopTaskSource;

        public TelemetryLoadController(ITelemetryRepositoryFactory telemetryRepositoryFactory, ISettingsProvider settingsProvider, ITelemetryViewsSynchronization telemetryViewsSynchronization )
        {
            _cachedTelemetries = new ConcurrentDictionary<string, LapTelemetryDto>();
            _loadedSessions = new List<string>();
            _knownLaps = new List<string>();
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

        public async Task<IReadOnlyCollection<SessionInfoDto>> GetAllArchivedSessionInfoAsync()
        {
            try
            {
                return await Task.Run(() => _telemetryRepository.GetAllArchivedSessions());
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while loading archived sessions");
            }

            return Enumerable.Empty<SessionInfoDto>().ToList().AsReadOnly();
        }

        public async Task RefreshLoadedSessions()
        {
            IReadOnlyCollection<SessionInfoDto> sessions = null;
            await Task.Run(() => sessions = _telemetryRepository.LoadPreviouslyLoadedSessions(_loadedSessions));

            if (sessions == null)
            {
                return;
            }

            List<LapSummaryDto> allLaps = sessions.SelectMany(x => x.LapsSummary).Where(y => !_knownLaps.Contains(y.Id)).ToList();
            foreach (LapSummaryDto lapSummaryDto in allLaps)
            {
                FillCustomDisplayName(lapSummaryDto);
                _knownLaps.Add(lapSummaryDto.Id);
                _telemetryViewsSynchronization.NotifyLappAddedToSession(lapSummaryDto);
            }
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
            if (!_loadedSessions.Contains(sessionInfoDto.Id))
            {
                _loadedSessions.Add(sessionInfoDto.Id);
                sessionInfoDto.LapsSummary.ForEach(x => _knownLaps.Add(x.Id));
            }

            sessionInfoDto.LapsSummary.ForEach(FillCustomDisplayName);
            _telemetryViewsSynchronization.NotifyNewSessionLoaded(sessionInfoDto);
            return sessionInfoDto;
        }

        public Task<SessionInfoDto> AddRecentSessionAsync(SessionInfoDto sessionInfoDto)
        {
            if (!_loadedSessions.Contains(sessionInfoDto.Id))
            {
                _loadedSessions.Add(sessionInfoDto.Id);
                sessionInfoDto.LapsSummary.ForEach(x => _knownLaps.Add(x.Id));
            }
            sessionInfoDto.LapsSummary.ForEach(FillCustomDisplayName);
            _telemetryViewsSynchronization.NotifySessionAdded(sessionInfoDto);
            return Task.FromResult(sessionInfoDto);
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

        public async Task<LapTelemetryDto> LoadLap(FileInfo file, string customDisplayName)
        {
            try
            {
                AddToActiveLapJob();
                if (!_cachedTelemetries.TryGetValue(file.FullName, out LapTelemetryDto lapTelemetryDto))
                {
                    lapTelemetryDto = await Task.Run(() => _telemetryRepository.LoadLapTelemetryDto(file));
                    lapTelemetryDto.LapSummary.CustomDisplayName = customDisplayName;
                    _cachedTelemetries[lapTelemetryDto.LapSummary.Id] = lapTelemetryDto;
                }
                _telemetryViewsSynchronization.NotifyLappAddedToSession(lapTelemetryDto.LapSummary);
                RemoveFromActiveLapJob();
                return lapTelemetryDto;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while loading lap telemetry");
                return null;
            }
        }

        public Task UnloadLap(LapSummaryDto lapSummaryDto)
        {
            _telemetryViewsSynchronization.NotifyLapUnloaded(lapSummaryDto);
            return Task.CompletedTask;
        }

        public async Task ArchiveSession(SessionInfoDto sessionInfoDto)
        {
            await _telemetryRepository.ArchiveSessions(sessionInfoDto);
        }

        public async Task OpenSessionFolder(SessionInfoDto sessionInfoDto)
        {
            await _telemetryRepository.OpenSessionFolder(sessionInfoDto);
        }

        public void DeleteSession(SessionInfoDto sessionInfoDto)
        {
            _telemetryRepository.DeleteSession(sessionInfoDto);
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
            _loadedSessions.ForEach( x => _telemetryRepository.CloseSession(x));
            _loadedSessions.Clear();
            _knownLaps.Clear();
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

        private async Task RefreshLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await RefreshLoadedSessions();
                await Task.Delay(5000, cancellationToken);
            }
        }

        public Task StartControllerAsync()
        {
            if (_loopTask != null)
            {
                return Task.CompletedTask;
            }

            _loopTaskSource = new CancellationTokenSource();
            _loopTask = RefreshLoop(_loopTaskSource.Token);
            return Task.CompletedTask;
        }

        public async Task StopControllerAsync()
        {
            if (_loopTask == null)
            {
                return;
            }

            try
            {
                _loopTaskSource.Cancel();
                await _loopTask;
            }
            catch (TaskCanceledException)
            {

            }
            finally
            {
                _cachedTelemetries.Clear();
            }
        }
    }
}