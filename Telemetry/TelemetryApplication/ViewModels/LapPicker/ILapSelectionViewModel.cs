namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LapPicker
{
    using System;
    using System.Collections.ObjectModel;
    using SecondMonitor.ViewModels;

    public interface ILapSelectionViewModel : IAbstractViewModel
    {
        ObservableCollection<ILapSummaryViewModel> LapSummaries { get; }

        ILapSummaryViewModel Selected { get; set; }

        string CarName { get; set; }

        string TrackName { get; set; }

        DateTime SessionTime { get; set; }

        string SimulatorName { get; set; }

        void AddLapSummaryViewModel(ILapSummaryViewModel lapSummaryViewModel);

        void Clear();
    }
}