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
        private bool _isBusy;

        public MainWindowViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            LapSelectionViewModel = viewModelFactory.Create<ILapSelectionViewModel>();
            SnapshotSectionViewModel = viewModelFactory.Create<ISnapshotSectionViewModel>();
            MapViewViewModel = viewModelFactory.Create<IMapViewViewModel>();
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }
        public ILapSelectionViewModel LapSelectionViewModel { get; }
        public ISnapshotSectionViewModel SnapshotSectionViewModel { get; }
        public IMapViewViewModel MapViewViewModel { get; }
    }
}