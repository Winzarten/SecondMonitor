namespace SecondMonitor.Rating.Application.Controller.RaceObserver
{
    using System.Linq;
    using System.Threading.Tasks;
    using DataModel.Extensions;
    using DataModel.Snapshot;
    using DataModel.Summary;
    using SimulatorRating;
    using States;
    using ViewModels;

    public class RaceObserverController : IRaceObserverController
    {
        private readonly ISimulatorRatingControllerFactory _simulatorRatingControllerFactory;
        private readonly IRaceStateFactory _raceStateFactory;
        private string _currentSimulator;
        private string _currentClass;
        private ISimulatorRatingController _simulatorRatingController;
        private IRaceState _currentState;

        public RaceObserverController(ISimulatorRatingControllerFactory simulatorRatingControllerFactory, IRaceStateFactory raceStateFactory)
        {
            _simulatorRatingControllerFactory = simulatorRatingControllerFactory;
            _raceStateFactory = raceStateFactory;
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

        public async Task NotifySessionCompletion(SessionSummary sessionSummary)
        {
            if (_currentState != null && await _currentState.DoSessionCompletion(sessionSummary))
            {
                _currentState = _currentState.GetNextState();
            }
            RefreshViewModelByState();
        }

        public async Task NotifyDataLoaded(SimulatorDataSet simulatorDataSet)
        {
            await CheckSimulatorClassChange(simulatorDataSet);
            if (_currentState != null && await _currentState.DoDataLoaded(simulatorDataSet))
            {
                _currentState = _currentState.GetNextState();
            }
            RefreshViewModelByState();
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
            _currentState = _raceStateFactory.CreateInitialState(_currentSimulator);
            RefreshClassesOnVm();
            RefreshSimulatorRatingOnVm();
        }

        private void RefreshClassesOnVm()
        {
            RatingApplicationViewModel.ClearSelectableClasses();
            _simulatorRatingController.GetAllKnowClassNames().OrderBy(x => x).ForEach(RatingApplicationViewModel.AddSelectableClass);
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

        private void RefreshViewModelByState()
        {
            if (_currentState == null)
            {
                return;
            }

            RatingApplicationViewModel.SessionPhaseKind = _currentState.SessionPhaseKind;
            RatingApplicationViewModel.SessionKind = _currentState.SessionKind;
        }
    }
}