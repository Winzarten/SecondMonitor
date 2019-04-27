namespace SecondMonitor.Rating.Application.Controller.RaceObserver
{
    using System.Threading.Tasks;
    using DataModel.Snapshot;
    using DataModel.Summary;
    using SimulatorRating;
    using ViewModels;

    public class RaceObserverController : IRaceObserverController
    {
        private readonly ISimulatorRatingControllerFactory _simulatorRatingControllerFactory;
        private string _currentSimulator;
        private string _currentClass;
        private ISimulatorRatingController _simulatorRatingController;

        public RaceObserverController(ISimulatorRatingControllerFactory simulatorRatingControllerFactory)
        {
            _simulatorRatingControllerFactory = simulatorRatingControllerFactory;
            _currentSimulator = string.Empty;
            _currentClass = string.Empty;
        }

        public Task StartControllerAsync()
        {
            return Task.CompletedTask;
        }

        public async Task StopControllerAsync()
        {
            if (_simulatorRatingController != null)
            {
                await _simulatorRatingController.StopControllerAsync();
            }

        }

        public IRatingApplicationViewModel RatingApplicationViewModel { get; set; }

        public Task NotifySessionCompletion(SessionSummary sessionSummary)
        {
            return Task.CompletedTask;
        }

        public async Task NotifyDataLoaded(SimulatorDataSet simulatorDataSet)
        {
            await CheckSimulatorClassChange(simulatorDataSet);
        }

        private async Task CheckSimulatorClassChange(SimulatorDataSet simulatorDataSet)
        {
            if (simulatorDataSet.Source != _currentSimulator && !string.IsNullOrWhiteSpace(simulatorDataSet.Source))
            {
                _currentSimulator = simulatorDataSet.Source;
                _currentClass = string.Empty;
                await OnSimulatorChanged();
                OnClassChanged();
                return;
            }

            if (simulatorDataSet?.PlayerInfo == null)
            {
                return;
            }

            if (simulatorDataSet.PlayerInfo.CarClassName == _currentClass || string.IsNullOrWhiteSpace(simulatorDataSet.PlayerInfo.CarClassName))
            {
                return;
            }

            _currentClass = simulatorDataSet.PlayerInfo.CarClassName;
            OnClassChanged();
        }

        private void OnClassChanged()
        {
            RefreshClassRatingOnVm();
        }

        private async Task OnSimulatorChanged()
        {
            if (_simulatorRatingController != null)
            {
                await _simulatorRatingController.StopControllerAsync();
            }

            _simulatorRatingController = null;
            if (!_simulatorRatingControllerFactory.IsSimulatorSupported(_currentSimulator))
            {
                return;
            }

            _simulatorRatingController = _simulatorRatingControllerFactory.CreateController(_currentSimulator);
            await _simulatorRatingController.StartControllerAsync();
            RefreshSimulatorRatingOnVm();
        }

        private void RefreshSimulatorRatingOnVm()
        {
            if (_simulatorRatingController == null)
            {
                return;
            }
            RatingApplicationViewModel.SimulatorRating.FromModel(_simulatorRatingController.GetPlayerOverallRating());
        }

        private void RefreshClassRatingOnVm()
        {
            if (string.IsNullOrWhiteSpace(_currentClass))
            {
                return;
            }

            RatingApplicationViewModel.ClassRating.FromModel(_simulatorRatingController.GetPlayerRating(_currentClass));
        }
    }
}