namespace SecondMonitor.Rating.Application.Controller
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Common.Repository;
    using DataModel.Snapshot;
    using DataModel.Summary;
    using NLog;
    using RaceObserver;
    using SecondMonitor.ViewModels.Factory;
    using ViewModels;

    public class RatingApplicationController : IRatingApplicationController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Stopwatch _refreshStopwatch;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IRaceObserverController _raceObserverController;

        public RatingApplicationController(IViewModelFactory viewModelFactory, IRaceObserverController raceObserverController)
        {
            _refreshStopwatch = Stopwatch.StartNew();
            _viewModelFactory = viewModelFactory;
            _raceObserverController = raceObserverController;
        }

        public IRatingApplicationViewModel RatingApplicationViewModel { get; set; }

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
                await _raceObserverController.NotifyDataLoaded(simulatorDataSet);
                _refreshStopwatch.Restart();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}