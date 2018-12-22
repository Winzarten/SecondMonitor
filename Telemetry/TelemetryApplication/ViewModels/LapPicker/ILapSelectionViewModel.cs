namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LapPicker
{
    using System;
    using System.Collections.ObjectModel;
    using Controllers.Synchronization;
    using SecondMonitor.ViewModels;

    public interface ILapSelectionViewModel : IAbstractViewModel
    {

        event EventHandler<LapSummaryArgs> LapSelected;
        event EventHandler<LapSummaryArgs> LapUnselected;

        ObservableCollection<ILapSummaryViewModel> LapSummaries { get; }

        string CarName { get; set; }

        string TrackName { get; set; }

        DateTime SessionTime { get; set; }

        string SimulatorName { get; set; }

        void AddLapSummaryViewModel(ILapSummaryViewModel lapSummaryViewModel);

        void Clear();
    }
}