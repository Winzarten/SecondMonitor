namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Factory;
    using LapPicker;
    using MapView;
    using Settings;
    using Snapshot;
    using Synchronization;
    using TelemetryLoad;
    using ViewModels;

    public class MainWindowController : IMainWindowController
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly ITelemetryLoadController _telemetryLoadController;
        private readonly ILapPickerController _lapPickerController;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IMainWindowViewModel _mainWindowViewModel;
        private readonly ISnapshotSectionController _snapshotSectionController;
        private readonly IMapViewController _mapViewController;
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;

        public MainWindowController(ISettingsProvider settingsProvider, ITelemetryLoadController telemetryLoadController, ILapPickerController lapPickerController, IViewModelFactory viewModelFactory, IMainWindowViewModel mainWindowViewModel,
            ISnapshotSectionController snapshotSectionController, IMapViewController mapViewController, ITelemetryViewsSynchronization telemetryViewsSynchronization)
        {
            _settingsProvider = settingsProvider;
            _telemetryLoadController = telemetryLoadController;
            _lapPickerController = lapPickerController;
            _viewModelFactory = viewModelFactory;
            _mainWindowViewModel = mainWindowViewModel;
            _snapshotSectionController = snapshotSectionController;
            _mapViewController = mapViewController;
            _telemetryViewsSynchronization = telemetryViewsSynchronization;

            _snapshotSectionController.MainWindowViewModel = _mainWindowViewModel;
            _mapViewController.MapViewViewModel = _mainWindowViewModel.MapViewViewModel;
        }

        public Window MainWindow { get; set; }

        public async Task LoadTelemetrySession(string telemetryIdentifier)
        {
            await _telemetryLoadController.LoadSessionAsync(telemetryIdentifier);
        }

        public async Task LoadLastTelemetrySession()
        {
            await _telemetryLoadController.LoadLastSessionAsync();
        }


        public void StartController()
        {
            MainWindow.DataContext = _mainWindowViewModel;
            ShowMainWindow();
            StartChildControllers();
        }

        public void StopController()
        {
            StopChildControllers();
        }

        private void StartChildControllers()
        {
            Subscribe();
            _lapPickerController.StartController();
            _snapshotSectionController.StartController();
            _mapViewController.StartController();
        }

        private void StopChildControllers()
        {
            _lapPickerController.StopController();
            _snapshotSectionController.StopController();
            _mapViewController.StopController();;
            UnSubscribe();
        }

        private void ShowMainWindow()
        {
            MainWindow.Show();
        }

        private void Subscribe()
        {
            _telemetryViewsSynchronization.LapLoadingStarted += TelemetryViewsSynchronizationOnLapLoadingStarted;
            _telemetryViewsSynchronization.LapLoadingFinished += _telemetryViewsSynchronization_LapLoadingFinished;
        }

        private void UnSubscribe()
        {
            _telemetryViewsSynchronization.LapLoadingStarted -= TelemetryViewsSynchronizationOnLapLoadingStarted;
            _telemetryViewsSynchronization.LapLoadingFinished -= _telemetryViewsSynchronization_LapLoadingFinished;
        }

        private void _telemetryViewsSynchronization_LapLoadingFinished(object sender, EventArgs e)
        {
            _mainWindowViewModel.IsBusy = false;
        }

        private void TelemetryViewsSynchronizationOnLapLoadingStarted(object sender, EventArgs e)
        {
            _mainWindowViewModel.IsBusy = true;
        }
    }
}