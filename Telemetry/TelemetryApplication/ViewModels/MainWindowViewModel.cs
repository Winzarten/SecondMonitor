namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels
{
    using Factory;
    using LapPicker;
    using SecondMonitor.ViewModels;
    using SnapshotSection;

    public class MainWindowViewModel : AbstractViewModel, IMainWindowViewModel
    {
        private readonly IViewModelFactory _viewModelFactory;

        public MainWindowViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            LapSelectionViewModel = viewModelFactory.Create<ILapSelectionViewModel>();
            SnapshotSectionViewModel = viewModelFactory.Create<ISnapshotSectionViewModel>();
        }

        public ILapSelectionViewModel LapSelectionViewModel { get; }
        public ISnapshotSectionViewModel SnapshotSectionViewModel { get; }
    }
}