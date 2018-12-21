namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LapPicker
{
    using System.Collections.ObjectModel;
    using SecondMonitor.ViewModels;

    public interface ILapSelectionViewModel : IAbstractViewModel
    {
        ObservableCollection<ILapSummaryViewModel> LapSummaries { get; }

        void AddLapSummaryViewModel(ILapSummaryViewModel lapSummaryViewModel);

        void Clear();
    }
}