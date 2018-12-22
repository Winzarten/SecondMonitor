namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.Replay
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;
    using Factory;
    using Settings;
    using Synchronization;
    using TelemetryManagement.DTO;
    using TelemetryManagement.StoryBoard;
    using ViewModels.Replay;
    using ViewModels.SnapshotSection;

    public class ReplayController : IReplayController
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;
        private readonly ISettingsProvider _settingsProvider;
        private readonly TelemetryStoryBoardFactory _telemetryStoryBoardFactory;
        private readonly Dictionary<string, TelemetryStoryboard> _storyboards;
        private LapSummaryDto _mainLap;
        private IReplayViewModel _replayViewModel;

        public ReplayController(IViewModelFactory viewModelFactory, ITelemetryViewsSynchronization telemetryViewsSynchronization, ISettingsProvider settingsProvider, TelemetryStoryBoardFactory telemetryStoryBoardFactory)
        {
            _storyboards = new Dictionary<string, TelemetryStoryboard>();
            _viewModelFactory = viewModelFactory;
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _settingsProvider = settingsProvider;
            _telemetryStoryBoardFactory = telemetryStoryBoardFactory;
        }

        public LapSummaryDto MainLap
        {
            get => _mainLap;
            set
            {
                _mainLap = value;
                SelectedValueChanged();
            }
        }

        public ISnapshotSectionViewModel SnapshotSectionViewModel
        {
            set => _replayViewModel = value.ReplayViewModel;
        }

        public void StartController()
        {
            Subscribe();
        }

        public void StopController()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            _replayViewModel.PropertyChanged += ReplayViewModelOnPropertyChanged;
            _telemetryViewsSynchronization.NewSessionLoaded += TelemetryViewsSynchronizationOnNewSessionLoaded;
            _telemetryViewsSynchronization.LapLoaded += TelemetryViewsSynchronizationOnLapLoaded;
            _telemetryViewsSynchronization.LapUnloaded += TelemetryViewsSynchronizationOnLapUnloaded;
            _telemetryViewsSynchronization.SyncTelemetryView += TelemetryViewsSynchronizationOnSyncTelemetryView;
        }

        private void UnSubscribe()
        {
            _replayViewModel.PropertyChanged -= ReplayViewModelOnPropertyChanged;
            _telemetryViewsSynchronization.NewSessionLoaded -= TelemetryViewsSynchronizationOnNewSessionLoaded;
            _telemetryViewsSynchronization.LapLoaded -= TelemetryViewsSynchronizationOnLapLoaded;
            _telemetryViewsSynchronization.LapUnloaded -= TelemetryViewsSynchronizationOnLapUnloaded;
            _telemetryViewsSynchronization.SyncTelemetryView -= TelemetryViewsSynchronizationOnSyncTelemetryView;
        }

        private void TelemetryViewsSynchronizationOnSyncTelemetryView(object sender, TelemetrySnapshotArgs e)
        {
            if (e.LapSummaryDto.Id != MainLap?.Id || _replayViewModel == null)
            {
                return;
            }

            UpdateViewModels(e.TelemetrySnapshot);
        }

        private void UpdateViewModels(TimedTelemetrySnapshot telemetrySnapshot)
        {
            _replayViewModel.DisplayTime = telemetrySnapshot.LapTime;
            _replayViewModel.DisplayDistance = Distance.FromMeters(telemetrySnapshot.PlayerData.LapDistance);
        }

        private void ReplayViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ReplayViewModel.SelectedDistance))
            {
                SelectedValueChanged();
            }
        }

        private void SelectedValueChanged()
        {
            if (MainLap == null || _replayViewModel?.TrackLength == null || !_storyboards.ContainsKey(MainLap.Id))
            {
                return;
            }
            Distance selectedDistance = Distance.CreateByUnits(_replayViewModel.SelectedDistance, _replayViewModel.DistanceUnits);
            TelemetryStoryboard storyboard = _storyboards[MainLap.Id];
            TelemetryFrame closestFrame = storyboard.FindFrameByDistance(selectedDistance);
            _telemetryViewsSynchronization.NotifySynchronizeToSnapshot(closestFrame.TelemetrySnapshot, storyboard.LapSummaryDto);
        }

        private void RefreshViewModelBasicInfo(SessionInfoDto sessionInfoDto)
        {
            _replayViewModel.DistanceUnits = _settingsProvider.DisplaySettingsViewModel.DistanceUnitsSmall;
            _replayViewModel.TrackLength = Distance.FromMeters(sessionInfoDto.LayoutLength);
        }

        private void TelemetryViewsSynchronizationOnNewSessionLoaded(object sender, TelemetrySessionArgs e)
        {
            RefreshViewModelBasicInfo(e.SessionInfoDto);
        }

        private void TelemetryViewsSynchronizationOnLapUnloaded(object sender, LapSummaryArgs e)
        {
            RemoveStoryBoardFromCache(e.LapSummary);
        }

        private async void TelemetryViewsSynchronizationOnLapLoaded(object sender, LapTelemetryArgs e)
        {
            _replayViewModel.IsEnabled = false;
            await CreateStoryBoard(e.LapTelemetry);
            _replayViewModel.IsEnabled = true;
            if (e.LapTelemetry.LapSummary == MainLap)
            {
                SelectedValueChanged();
            }
        }

        private async Task CreateStoryBoard(LapTelemetryDto lapTelemetryDto)
        {
            RemoveStoryBoardFromCache(lapTelemetryDto.LapSummary);

            TelemetryStoryboard storyboard = null;
            await Task.Run(() => storyboard = _telemetryStoryBoardFactory.Create(lapTelemetryDto));
            _storyboards[lapTelemetryDto.LapSummary.Id] = storyboard;
        }

        private void RemoveStoryBoardFromCache(LapSummaryDto lapSummaryDto)
        {
            if (_storyboards.ContainsKey(lapSummaryDto.Id))
            {
                _storyboards.Remove(lapSummaryDto.Id);
            }
        }
    }
}