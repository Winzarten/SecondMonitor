namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LapPicker
{
    using System.Collections.ObjectModel;
    using SecondMonitor.ViewModels;

    public class LapSelectionViewModel : AbstractViewModel, ILapSelectionViewModel
    {

        public LapSelectionViewModel()
        {
            LapSummaries = new ObservableCollection<ILapSummaryViewModel>();
        }

        public ObservableCollection<ILapSummaryViewModel> LapSummaries { get; }

        public void AddLapSummaryViewModel(ILapSummaryViewModel lapSummaryViewModel)
        {
            LapSummaries.Add(lapSummaryViewModel);
        }

        public void Clear()
        {
            LapSummaries.Clear();
        }
    }
}