namespace SecondMonitor.Rating.Application.Controller
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using DataModel.Snapshot;
    using DataModel.Summary;
    using NLog;
    using RaceObserver;
    using RatingProvider;
    using SecondMonitor.ViewModels.Factory;
    using SecondMonitor.ViewModels.Settings;
    using SecondMonitor.ViewModels.Settings.ViewModel;
    using ViewModels;

    public class RatingApplicationController : IRatingApplicationController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Stopwatch _refreshStopwatch;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IRaceObserverController _raceObserverController;
        private bool _inMp;
        private readonly DisplaySettingsViewModel _displaySettingsViewModel;

        public RatingApplicationController(IViewModelFactory viewModelFactory, IRaceObserverController raceObserverController, ISettingsProvider settingsProvider)
        {
            _refreshStopwatch = Stopwatch.StartNew();
            _viewModelFactory = viewModelFactory;
            _raceObserverController = raceObserverController;
            _displaySettingsViewModel = settingsProvider.DisplaySettingsViewModel;
        }

        public IRatingApplicationViewModel RatingApplicationViewModel { get; set; }
        public IRatingProvider RatingProvider => _raceObserverController;

        public async Task StartControllerAsync()
        {
            RatingApplicationViewModel = _viewModelFactory.Create<IRatingApplicationViewModel>();
            RatingApplicationViewModel.IsVisible = _displaySettingsViewModel.RatingSettingsViewModel.IsEnabled;
            _raceObserverController.RatingApplicationViewModel = RatingApplicationViewModel;
            _displaySettingsViewModel.RatingSettingsViewModel.PropertyChanged+= RatingSettingsViewModelOnPropertyChanged;
            RatingApplicationViewModel.PropertyChanged += RatingApplicationViewModelOnPropertyChanged;
            await _raceObserverController.StartControllerAsync();
        }

        private void RatingApplicationViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RatingApplicationViewModel.IsRateRaceCheckboxChecked))
            {
                _raceObserverController.Reset();
            }
        }

        private void RatingSettingsViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RatingSettingsViewModel.IsEnabled))
            {
                RatingApplicationViewModel.IsVisible = _displaySettingsViewModel.RatingSettingsViewModel.IsEnabled;
                _raceObserverController.Reset();
            }
        }

        public async Task StopControllerAsync()
        {
            RatingApplicationViewModel.PropertyChanged -= RatingApplicationViewModelOnPropertyChanged;
            _displaySettingsViewModel.RatingSettingsViewModel.PropertyChanged -= RatingSettingsViewModelOnPropertyChanged;
            await _raceObserverController.StopControllerAsync();
        }

        public async Task NotifySessionCompletion(SessionSummary sessionSummary)
        {
            if (_inMp || !_displaySettingsViewModel.RatingSettingsViewModel.IsEnabled || !RatingApplicationViewModel.IsRateRaceCheckboxChecked)
            {
                return;
            }
            try
            {
                await _raceObserverController.NotifySessionCompletion(sessionSummary);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public async Task NotifyDataLoaded(SimulatorDataSet simulatorDataSet)
        {
            if (_refreshStopwatch.ElapsedMilliseconds < 1000)
            {
                return;
            }
            try
            {
                _refreshStopwatch.Restart();
                CheckMp(simulatorDataSet.SessionInfo);
                if (_inMp || !_displaySettingsViewModel.RatingSettingsViewModel.IsEnabled || !RatingApplicationViewModel.IsRateRaceCheckboxChecked)
                {
                    return;
                }
                await _raceObserverController.NotifyDataLoaded(simulatorDataSet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void CheckMp(SessionInfo sessionInfo)
        {
            if (_inMp == sessionInfo.IsMultiplayer)
            {
                return;
            }

            if (sessionInfo.IsMultiplayer)
            {
                _inMp = true;
                RatingApplicationViewModel.CollapsedMessage = "MP Detected, Rating Disabled";
                RatingApplicationViewModel.IsCollapsed = false;
            }
            else
            {
                _inMp = false;
                RatingApplicationViewModel.CollapsedMessage = string.Empty;
                RatingApplicationViewModel.IsCollapsed = true;
            }

        }
    }
}