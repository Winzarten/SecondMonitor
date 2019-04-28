namespace SecondMonitor.Rating.Application.ViewModels
{
    using System.Collections.ObjectModel;
    using Common.DataModel.Player;
    using Controller.RaceObserver.States;
    using Rating;
    using SecondMonitor.ViewModels;

    public interface IRatingApplicationViewModel : IViewModel
    {
        IRatingViewModel SimulatorRating { get; }
        IRatingViewModel ClassRating { get; }
        SessionKind SessionKind { get; set; }
        SessionPhaseKind SessionPhaseKind { get; set; }
        ObservableCollection<string> SelectableClasses { get; }
        string SelectedClass { get; set; }

        void ApplySimulatorRating(DriversRating driversRating);
        void ApplyClassRating(DriversRating driversRating);

        void AddSelectableClass(string className);
        void ClearSelectableClasses();

    }
}