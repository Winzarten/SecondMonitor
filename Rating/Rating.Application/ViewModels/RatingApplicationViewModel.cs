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

        public RatingApplicationViewModel(IViewModelFactory viewModelFactory)
        {
            SimulatorRating = viewModelFactory.Create<IRatingViewModel>();
            ClassRating = viewModelFactory.Create<IRatingViewModel>();
            SelectableClasses = new ObservableCollection<string>();
        }

        public IRatingViewModel SimulatorRating { get; }


        public IRatingViewModel ClassRating { get; }

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
        public string SelectedClass { get; set; }

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
    }
}