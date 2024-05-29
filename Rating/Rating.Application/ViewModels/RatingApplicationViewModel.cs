namespace SecondMonitor.Rating.Application.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using Common.DataModel.Player;
    using Controller.RaceObserver.States;
    using Rating;
    using SecondMonitor.ViewModels;
    using SecondMonitor.ViewModels.Factory;

    public class RatingApplicationViewModel : AbstractViewModel, IRatingApplicationViewModel
    {
        private SessionKind _sessionKind;
        private SessionPhaseKind _sessionPhaseKind;
        private string _selectedClass;
        private bool _isClassSelectionEnable;
        private int _difficulty;
        private bool _useSuggestedDifficulty;
        private bool _isCollapsed;
        private string _invisibleMessage;
        private string _sessionTextInformation;
        private bool _isVisible;
        private bool _isRateRaceCheckboxChecked;

        public RatingApplicationViewModel(IViewModelFactory viewModelFactory)
        {
            SimulatorRating = viewModelFactory.Create<IRatingViewModel>();
            ClassRating = viewModelFactory.Create<IRatingViewModel>();
            SelectableClasses = new ObservableCollection<string>();
            AiLevels = new ObservableCollection<int>();
            UseSuggestedDifficulty = true;
            IsCollapsed = true;
            IsRateRaceCheckboxChecked = true;
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        public bool IsCollapsed
        {
            get => _isCollapsed;
            set => SetProperty(ref _isCollapsed, value);
        }

        public string CollapsedMessage
        {
            get => _invisibleMessage;
            set => SetProperty(ref _invisibleMessage, value);
        }

        public IRatingViewModel SimulatorRating { get; }


        public IRatingViewModel ClassRating { get; }

        public string SessionTextInformation
        {
            get => _sessionTextInformation;
            set => SetProperty(ref _sessionTextInformation, value);
        }

        public SessionKind SessionKind
        {
            get => _sessionKind;
            set => SetProperty(ref _sessionKind, value);
        }

        public SessionPhaseKind SessionPhaseKind
        {
            get => _sessionPhaseKind;
            set => SetProperty(ref _sessionPhaseKind, value);
        }

        public ObservableCollection<string> SelectableClasses { get; set; }
        public ObservableCollection<int> AiLevels { get; set; }

        public string SelectedClass
        {
            get => _selectedClass;
            set => SetProperty(ref _selectedClass, value);
        }

        public bool IsClassSelectionEnable
        {
            get => _isClassSelectionEnable;
            set => SetProperty(ref _isClassSelectionEnable, value);
        }

        public int Difficulty
        {
            get => _difficulty;
            set => SetProperty(ref _difficulty, value);
        }

        public bool UseSuggestedDifficulty
        {
            get => _useSuggestedDifficulty;
            set => SetProperty(ref _useSuggestedDifficulty, value);
        }

        public bool IsRateRaceCheckboxChecked
        {
            get => _isRateRaceCheckboxChecked;
            set => SetProperty(ref _isRateRaceCheckboxChecked, value);
        }

        public void ApplySimulatorRating(DriversRating driversRating)
        {
            SimulatorRating.FromModel(driversRating);
        }

        public void ApplyClassRating(DriversRating driversRating)
        {
            ClassRating.FromModel(driversRating);
        }

        public void AddSelectableClass(string className)
        {
            if (SelectableClasses.Contains(className))
            {
                return;
            }

            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => SelectableClasses.Add(className));
                return;
            }

            SelectableClasses.Add(className);
        }

        public void ClearSelectableClasses()
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(ClearSelectableClasses);
                return;
            }
            SelectableClasses.Clear();
        }

        public void InitializeAiDifficultySelection(int minimumLevel, int maximumLevel)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => InitializeAiDifficultySelection(minimumLevel, maximumLevel));
                return;
            }
            AiLevels.Clear();
            for (int i = minimumLevel; i <= maximumLevel; i++)
            {
                AiLevels.Add(i);
            }
        }
    }
}