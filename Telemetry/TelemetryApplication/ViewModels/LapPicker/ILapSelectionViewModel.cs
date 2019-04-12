namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LapPicker
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using Controllers.Synchronization;
    using OpenWindow;
    using SecondMonitor.ViewModels;
    using SettingsWindow;

    public interface ILapSelectionViewModel : IViewModel
    {

        event EventHandler<LapSummaryArgs> LapSelected;
        event EventHandler<LapSummaryArgs> LapUnselected;

        IOpenWindowViewModel OpenWindowViewModel { get; set; }

        IOpenWindowViewModel AddWindowViewModel { get; set; }

        ISettingsWindowViewModel SettingsWindowViewModel { get; set; }

        ObservableCollection<ILapSummaryViewModel> LapSummaries { get; }

        ICommand AddCustomLapCommand { get; set; }

        ICommand OpenAggregatedChartSelectorCommand { get; set; }

        string CarName { get; set; }

        string TrackName { get; set; }

        string BestLap { get; set; }

        string BestSector1 { get; set; }

        string BestSector2 { get; set; }

        string BestSector3 { get; set; }

        DateTime SessionTime { get; set; }

        string SimulatorName { get; set; }

        void AddLapSummaryViewModel(ILapSummaryViewModel lapSummaryViewModel);

        void Clear();
    }
}