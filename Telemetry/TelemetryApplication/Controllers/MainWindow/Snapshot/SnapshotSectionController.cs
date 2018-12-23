namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.Snapshot
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Replay;
    using Settings;
    using Synchronization;
    using TelemetryManagement.DTO;
    using ViewModels;
    using ViewModels.SnapshotSection;

    public class SnapshotSectionController : ISnapshotSectionController
    {
        private readonly IReplayController _replayController;
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;
        private readonly ISettingsProvider _settingsProvider;
        private readonly List<IAbstractSnapshotViewModel> _abstractSnapshotViewModels;
        private ISnapshotSectionViewModel _snapshotSectionViewModel;
        private IMainWindowViewModel _mainWindowViewModel;
        private LapSummaryDto _mainLap;


        public SnapshotSectionController(IReplayController replayController, ITelemetryViewsSynchronization telemetryViewsSynchronization, ISettingsProvider settingsProvider)
        {
            _abstractSnapshotViewModels = new List<IAbstractSnapshotViewModel>();
            _replayController = replayController;
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _settingsProvider = settingsProvider;
        }

        public IMainWindowViewModel MainWindowViewModel
        {
            get => _mainWindowViewModel;
            set
            {
                _snapshotSectionViewModel = value.SnapshotSectionViewModel;
                _replayController.SnapshotSectionViewModel = _snapshotSectionViewModel;
                _snapshotSectionViewModel.PedalSectionViewModel.VelocityUnits = _settingsProvider.DisplaySettingsViewModel.VelocityUnits;
                _snapshotSectionViewModel.PressureUnits = _settingsProvider.DisplaySettingsViewModel.PressureUnits;
                _snapshotSectionViewModel.TemperatureUnits = _settingsProvider.DisplaySettingsViewModel.TemperatureUnits;
                _abstractSnapshotViewModels.Add(_snapshotSectionViewModel.PedalSectionViewModel);
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
            _telemetryViewsSynchronization.SyncTelemetryView += TelemetryViewsSynchronizationOnSyncTelemetryView;
            _snapshotSectionViewModel.PropertyChanged += SnapshotSectionViewModelOnPropertyChanged;
        }

        private void Unsubscribe()
        {
            _telemetryViewsSynchronization.LapLoaded -= TelemetryViewsSynchronizationOnLapLoaded;
            _telemetryViewsSynchronization.LapUnloaded -= TelemetryViewsSynchronizationOnLapUnLoaded;
            _telemetryViewsSynchronization.NewSessionLoaded -= TelemetryViewsSynchronizationOnNewSessionLoaded;
            _telemetryViewsSynchronization.SyncTelemetryView -= TelemetryViewsSynchronizationOnSyncTelemetryView;
            _snapshotSectionViewModel.PropertyChanged -= SnapshotSectionViewModelOnPropertyChanged;
        }

        private void TelemetryViewsSynchronizationOnSyncTelemetryView(object sender, TelemetrySnapshotArgs e)
        {
            if (e.LapSummaryDto != _mainLap)
            {
                return;
            }
            _abstractSnapshotViewModels.ForEach(x => x.FromModel(e.TelemetrySnapshot));
            _snapshotSectionViewModel.CarWheelsViewModel.ApplyPlayerInfo(e.TelemetrySnapshot.PlayerData);
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

            _mainLap = _snapshotSectionViewModel.SelectedLap;
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