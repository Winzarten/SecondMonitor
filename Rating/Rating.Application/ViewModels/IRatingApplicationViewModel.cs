﻿namespace SecondMonitor.Rating.Application.ViewModels
{
    using System.Collections.ObjectModel;
    using Common.DataModel.Player;
    using Controller.RaceObserver.States;
    using Rating;
    using SecondMonitor.ViewModels;

    public interface IRatingApplicationViewModel : IViewModel
    {
        bool IsVisible { get; set; }
        bool IsCollapsed { get; set; }
        string CollapsedMessage { get; set; }

        IRatingViewModel SimulatorRating { get; }
        IRatingViewModel ClassRating { get; }
        string SessionTextInformation { get; set; }
        SessionKind SessionKind { get; set; }
        SessionPhaseKind SessionPhaseKind { get; set; }
        ObservableCollection<string> SelectableClasses { get; }
        string SelectedClass { get; set; }
        bool IsClassSelectionEnable { get; set; }
        int Difficulty { get; set; }
        bool UseSuggestedDifficulty { get; set; }
        bool IsRateRaceCheckboxChecked { get; }

        void ApplySimulatorRating(DriversRating driversRating);
        void ApplyClassRating(DriversRating driversRating);

        void AddSelectableClass(string className);
        void ClearSelectableClasses();
        void InitializeAiDifficultySelection(int minimumLevel, int maximumLevel);

    }
}