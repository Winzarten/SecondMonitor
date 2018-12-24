namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels
{
    using Factory;
    using LapPicker;
    using MapView;
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
            MapViewViewModel = viewModelFactory.Create<IMapViewViewModel>();
        }

        public ILapSelectionViewModel LapSelectionViewModel { get; }
        public ISnapshotSectionViewModel SnapshotSectionViewModel { get; }
        public IMapViewViewModel MapViewViewModel { get; }
    }
}