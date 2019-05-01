namespace SecondMonitor.Rating.Application.Controller
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using DataModel.Snapshot;
    using DataModel.Summary;
    using NLog;
    using RaceObserver;
    using RatingProvider;
    using SecondMonitor.ViewModels.Factory;
    using ViewModels;

    public class RatingApplicationController : IRatingApplicationController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Stopwatch _refreshStopwatch;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IRaceObserverController _raceObserverController;
        private bool _inMp;

        public RatingApplicationController(IViewModelFactory viewModelFactory, IRaceObserverController raceObserverController)
        {
            _refreshStopwatch = Stopwatch.StartNew();
            _viewModelFactory = viewModelFactory;
            _raceObserverController = raceObserverController;
        }

        public IRatingApplicationViewModel RatingApplicationViewModel { get; set; }
        public IRatingProvider RatingProvider => _raceObserverController;

        public async Task StartControllerAsync()
        {
            RatingApplicationViewModel = _viewModelFactory.Create<IRatingApplicationViewModel>();
            _raceObserverController.RatingApplicationViewModel = RatingApplicationViewModel;
            await _raceObserverController.StartControllerAsync();
        }

        public async Task StopControllerAsync()
        {
            await _raceObserverController.StopControllerAsync();
        }

        public async Task NotifySessionCompletion(SessionSummary sessionSummary)
        {
            if (_inMp)
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
                if (_inMp)
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
                RatingApplicationViewModel.InvisibleMessage = "MP Detected, Rating Disabled";
                RatingApplicationViewModel.IsVisible = false;
            }
            else
            {
                _inMp = false;
                RatingApplicationViewModel.InvisibleMessage = string.Empty;
                RatingApplicationViewModel.IsVisible = true;
            }

        }
    }
}