namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.Snapshot
{
    using System.ComponentModel;
    using Replay;
    using Synchronization;
    using ViewModels;
    using ViewModels.SnapshotSection;

    public class SnapshotSectionController : ISnapshotSectionController
    {
        private readonly IReplayController _replayController;
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;
        private ISnapshotSectionViewModel _snapshotSectionViewModel;
        private IMainWindowViewModel _mainWindowViewModel;

        public SnapshotSectionController(IReplayController replayController, ITelemetryViewsSynchronization telemetryViewsSynchronization)
        {
            _replayController = replayController;
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
        }

        public IMainWindowViewModel MainWindowViewModel
        {
            get => _mainWindowViewModel;
            set
            {
                _snapshotSectionViewModel = value.SnapshotSectionViewModel;
                _replayController.SnapshotSectionViewModel = _snapshotSectionViewModel;
                _mainWindowViewModel = value;
            }
        }

        public void StartController()
        {
            Subscribe();
            StartChildControllers();
        }

        public void StopController()
        {
            Unsubscribe();
            StopChildControllers();
        }

        private void Subscribe()
        {
            _telemetryViewsSynchronization.LapLoaded += TelemetryViewsSynchronizationOnLapLoaded;
            _telemetryViewsSynchronization.LapUnloaded += TelemetryViewsSynchronizationOnLapUnLoaded;
            _telemetryViewsSynchronization.NewSessionLoaded += TelemetryViewsSynchronizationOnNewSessionLoaded;
            _snapshotSectionViewModel.PropertyChanged += SnapshotSectionViewModelOnPropertyChanged;
        }

        private void Unsubscribe()
        {
            _telemetryViewsSynchronization.LapLoaded -= TelemetryViewsSynchronizationOnLapLoaded;
            _telemetryViewsSynchronization.LapUnloaded -= TelemetryViewsSynchronizationOnLapUnLoaded;
            _telemetryViewsSynchronization.NewSessionLoaded -= TelemetryViewsSynchronizationOnNewSessionLoaded;
            _snapshotSectionViewModel.PropertyChanged -= SnapshotSectionViewModelOnPropertyChanged;
        }

        private void TelemetryViewsSynchronizationOnLapLoaded(object sender, LapTelemetryArgs e)
        {
            _snapshotSectionViewModel.AddAvailableLap(e.LapTelemetry.LapSummary);
        }

        private void TelemetryViewsSynchronizationOnNewSessionLoaded(object sender, TelemetrySessionArgs e)
        {
            _snapshotSectionViewModel.ClearAvailableLaps();
        }

        private void TelemetryViewsSynchronizationOnLapUnLoaded(object sender, LapSummaryArgs e)
        {
            _snapshotSectionViewModel.RemoveAvailableLap(e.LapSummary);
        }

        private void SnapshotSectionViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SnapshotSectionViewModel.SelectedLap))
            {
                return;
            }

            _replayController.MainLap = _snapshotSectionViewModel.SelectedLap;

        }

        private void StartChildControllers()
        {
            _replayController.StartController();
        }

        private void StopChildControllers()
        {
            _replayController.StopController();
        }


    }
}