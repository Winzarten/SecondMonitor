namespace SecondMonitor.Rating.Application.Controller.RaceObserver
{
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.DataModel.Player;
    using DataModel;
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
        private IRatingApplicationViewModel _ratingApplicationViewModel;
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

            if (RatingApplicationViewModel != null)
            {
                RatingApplicationViewModel.PropertyChanged -= RatingApplicationViewModelOnPropertyChanged;
            }

        }

        public IRatingApplicationViewModel RatingApplicationViewModel
        {
            get => _ratingApplicationViewModel;
            set
            {
                UnsubscribeViewModel();
                _ratingApplicationViewModel = value;
                SubscribeViewModel();
            }
        }

        private void SubscribeViewModel()
        {
            if (RatingApplicationViewModel == null)
            {
                return;
            }
            RatingApplicationViewModel.PropertyChanged += RatingApplicationViewModelOnPropertyChanged;
        }

        private void UnsubscribeViewModel()
        {
            if (RatingApplicationViewModel == null)
            {
                return;
            }
            RatingApplicationViewModel.PropertyChanged -= RatingApplicationViewModelOnPropertyChanged;
        }

        private void RatingApplicationViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_simulatorRatingController == null)
            {
                return;
            }
            if (e.PropertyName == nameof(RatingApplicationViewModel.SelectedClass) && _currentState.CanUserSelectClass)
            {
                _currentClass = RatingApplicationViewModel.SelectedClass;
                OnClassChanged();
            }

            if (e.PropertyName == nameof(RatingApplicationViewModel.UseSuggestedDifficulty) && RatingApplicationViewModel.UseSuggestedDifficulty)
            {
                RatingApplicationViewModel.Difficulty = _simulatorRatingController.GetSuggestedDifficulty(_currentClass);
            }

            if (e.PropertyName == nameof(RatingApplicationViewModel.Difficulty))
            {
                _currentState.SharedContext.UserSelectedDifficulty = RatingApplicationViewModel.Difficulty;
            }

        }

        public Task NotifySessionCompletion(SessionSummary sessionSummary)
        {
            if (_currentState != null && _currentState.DoSessionCompletion(sessionSummary))
            {
                _currentState = _currentState.NextState;
            }
            RefreshViewModelByState();
            return Task.CompletedTask;
        }

        public async Task NotifyDataLoaded(SimulatorDataSet simulatorDataSet)
        {
            await CheckSimulatorClassChange(simulatorDataSet);
            if (_currentState != null && _currentState.DoDataLoaded(simulatorDataSet))
            {
                _currentState = _currentState.NextState;
            }
            RefreshViewModelByState();
        }

        private async Task CheckSimulatorClassChange(SimulatorDataSet simulatorDataSet)
        {
            if (simulatorDataSet.Source != _currentSimulator && !string.IsNullOrWhiteSpace(simulatorDataSet.Source) && !SimulatorsNameMap.IsNotConnected(simulatorDataSet.Source))
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

            if (simulatorDataSet.PlayerInfo.CarClassName == _currentClass || string.IsNullOrWhiteSpace(simulatorDataSet.PlayerInfo.CarClassName) || _currentState.CanUserSelectClass)
            {
                return;
            }

            _currentClass = simulatorDataSet.PlayerInfo.CarClassName;
            RatingApplicationViewModel.AddSelectableClass(_currentClass);
            RatingApplicationViewModel.SelectedClass = _currentClass;
            OnClassChanged();
        }

        private void OnClassChanged()
        {
            RefreshClassRatingOnVm();
        }

        private async Task OnSimulatorChanged()
        {
            if (!_simulatorRatingControllerFactory.IsSimulatorSupported(_currentSimulator))
            {
                RatingApplicationViewModel.IsVisible = false;
                RatingApplicationViewModel.InvisibleMessage = $"{_currentSimulator}, is not supported";
                return;
            }
            RatingApplicationViewModel.IsVisible = true;
            if (_simulatorRatingController != null)
            {
                await _simulatorRatingController.StopControllerAsync();
            }

            _simulatorRatingController = _simulatorRatingControllerFactory.CreateController(_currentSimulator);
            await _simulatorRatingController.StartControllerAsync();
            RatingApplicationViewModel.InitializeAiDifficultySelection(_simulatorRatingController.MinimumAiDifficulty, _simulatorRatingController.MaximumAiDifficulty);
            _currentState = _raceStateFactory.CreateInitialState(_currentSimulator);
            _currentState.SharedContext.UserSelectedDifficulty = RatingApplicationViewModel.Difficulty;
            _currentState.SharedContext.SimulatorRatingController = _simulatorRatingController;
            RefreshClassesOnVm();
            RefreshSimulatorRatingOnVm();
        }

        private void RefreshClassesOnVm()
        {
            RatingApplicationViewModel.ClearSelectableClasses();
            _simulatorRatingController.GetAllKnowClassNames().OrderBy(x => x).ForEach(RatingApplicationViewModel.AddSelectableClass);
            RatingApplicationViewModel.SelectedClass = RatingApplicationViewModel.SelectableClasses.First();
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
            if (RatingApplicationViewModel.UseSuggestedDifficulty)
            {
                RatingApplicationViewModel.Difficulty = _simulatorRatingController.GetSuggestedDifficulty(_currentClass);
            }
        }

        private void RefreshViewModelByState()
        {
            if (_currentState == null)
            {
                return;
            }

            RatingApplicationViewModel.IsClassSelectionEnable = _currentState.CanUserSelectClass;
            RatingApplicationViewModel.SessionPhaseKind = _currentState.SessionPhaseKind;
            RatingApplicationViewModel.SessionKind = _currentState.SessionKind;
            RatingApplicationViewModel.SessionTextInformation = _currentState.SessionDescription;
        }

        public bool TryGetRatingForDriverCurrentSession(string driverName, out DriversRating driversRating)
        {
            return _currentState.TryGetDriverRating(driverName, out driversRating);
        }
    }
}