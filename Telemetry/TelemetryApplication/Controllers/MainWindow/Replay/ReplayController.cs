﻿namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.Replay
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using WindowsControls.WPF.Commands;
    using DataModel.BasicProperties;
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
        private TelemetryFrame _displayedFrame;
        private CancellationTokenSource _playCancellationSource;
        private bool _propertyEventsEnabled;

        public ReplayController(IViewModelFactory viewModelFactory, ITelemetryViewsSynchronization telemetryViewsSynchronization, ISettingsProvider settingsProvider, TelemetryStoryBoardFactory telemetryStoryBoardFactory)
        {
            _propertyEventsEnabled = true;
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
                _replayViewModel.IsEnabled = _mainLap != null;
            }
        }

        public ISnapshotSectionViewModel SnapshotSectionViewModel
        {
            set => _replayViewModel = value.ReplayViewModel;
        }

        public void StartController()
        {
            BindCommands();
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
            if (e.LapSummaryDto.Id != MainLap?.Id || _replayViewModel == null || e.TelemetrySnapshot == _displayedFrame.TelemetrySnapshot)
            {
                return;
            }

            _displayedFrame = _storyboards[MainLap.Id].TelemetryFrames.FirstOrDefault(x => x.TelemetrySnapshot == e.TelemetrySnapshot);
            UpdateViewModels();
        }

        private void UpdateViewModels()
        {
            _propertyEventsEnabled = false;
            _replayViewModel.DisplayTime = _displayedFrame.FrameTime;
            _replayViewModel.DisplayDistance =_displayedFrame.FrameDistance;
            _replayViewModel.SelectedDistance = _displayedFrame.FrameDistance.GetByUnit(_replayViewModel.DistanceUnits);
            _propertyEventsEnabled = true;
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
            if (!_propertyEventsEnabled || MainLap == null || _replayViewModel?.TrackLength == null || !_storyboards.ContainsKey(MainLap.Id))
            {
                return;
            }
            Distance selectedDistance = Distance.CreateByUnits(_replayViewModel.SelectedDistance, _replayViewModel.DistanceUnits);
            TelemetryStoryboard storyboard = _storyboards[MainLap.Id];
            TelemetryFrame closestFrame = storyboard.FindFrameByDistance(selectedDistance);
            _displayedFrame = closestFrame;
            UpdateViewModels();
            FireSynchronization();
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

        private void BindCommands()
        {
            if (_replayViewModel == null)
            {
                return;;
            }

            _replayViewModel.PlayCommand = new AsyncCommand(Play);
            _replayViewModel.StopCommand = new RelayCommand(Stop);
            _replayViewModel.NextFrameCommand = new RelayCommand(NextFrame);
            _replayViewModel.PreviousFrameCommand = new RelayCommand(PreviousFrame);
        }

        private void PreviousFrame()
        {
            if (_displayedFrame == null || _playCancellationSource != null || _displayedFrame.PreviousFrame == null)
            {
                return;
            }

            _displayedFrame = _displayedFrame.PreviousFrame;
            FireSynchronization();
            UpdateViewModels();
        }

        private void NextFrame()
        {
            if (_displayedFrame == null || _playCancellationSource != null || _displayedFrame.NextFrame == null)
            {
                return;
            }

            _displayedFrame = _displayedFrame.NextFrame;
            FireSynchronization();
            UpdateViewModels();
        }

        private async Task Play()
        {
            if (_displayedFrame == null || _playCancellationSource != null)
            {
                return;
            }

            _playCancellationSource = new CancellationTokenSource();
            TelemetryFrame nextFrame = _displayedFrame.NextFrame;
            TimeSpan timeToWait = TimeSpan.Zero;
            Stopwatch sw = new Stopwatch();
            while (nextFrame != null && nextFrame != _displayedFrame && !_playCancellationSource.IsCancellationRequested)
            {
                timeToWait = nextFrame.FrameTime - _displayedFrame.FrameTime + (timeToWait - sw.Elapsed);
                sw.Restart();
                if (timeToWait > TimeSpan.Zero)
                {
                    await Task.Delay(timeToWait);
                }
                else
                {
                    await Task.Delay(10);
                }

                _displayedFrame = nextFrame;
                FireSynchronization();
                UpdateViewModels();
                nextFrame = _displayedFrame.NextFrame;
                //nextFrame = _displayedFrame.Forward(TimeSpan.FromMilliseconds(50));
            }

            _playCancellationSource = null;
        }

        private void FireSynchronization()
        {
            _telemetryViewsSynchronization.NotifySynchronizeToSnapshot(_displayedFrame.TelemetrySnapshot, _mainLap);
            foreach (TelemetryStoryboard otherLapStoryboard in _storyboards.Values.Where(x => x.LapSummaryDto.Id != _mainLap.Id))
            {
                TelemetryFrame otherLapFrame = otherLapStoryboard.FindFrameByTime(_displayedFrame.FrameTime);
                _telemetryViewsSynchronization.NotifySynchronizeToSnapshot(otherLapFrame.TelemetrySnapshot, otherLapStoryboard.LapSummaryDto);
            }
        }

        private void Stop()
        {
            _playCancellationSource?.Cancel(false);
        }
    }
}