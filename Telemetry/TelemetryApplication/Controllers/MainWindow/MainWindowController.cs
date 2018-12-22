namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Factory;
    using LapPicker;
    using Settings;
    using Snapshot;
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

        public MainWindowController(ISettingsProvider settingsProvider, ITelemetryLoadController telemetryLoadController, ILapPickerController lapPickerController, IViewModelFactory viewModelFactory, IMainWindowViewModel mainWindowViewModel, ISnapshotSectionController snapshotSectionController)
        {
            _settingsProvider = settingsProvider;
            _telemetryLoadController = telemetryLoadController;
            _lapPickerController = lapPickerController;
            _viewModelFactory = viewModelFactory;
            _mainWindowViewModel = mainWindowViewModel;
            _snapshotSectionController = snapshotSectionController;

            _snapshotSectionController.MainWindowViewModel = _mainWindowViewModel;
        }

        public Window MainWindow { get; set; }

        public async Task LoadTelemetrySession(string telemetryIdentifier)
        {
            await _telemetryLoadController.LoadSessionAsync(telemetryIdentifier);
        }

        public void StartController()
        {
            MainWindow.Closed+=MainWindowOnClosed;
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
            _lapPickerController.StartController();
            _snapshotSectionController.StartController();
        }

        private void StopChildControllers()
        {
            _lapPickerController.StopController();
            _snapshotSectionController.StopController();
        }

        private void MainWindowOnClosed(object sender, EventArgs e)
        {
            StopController();
            Application.Current.Shutdown();
        }

        private void ShowMainWindow()
        {
            MainWindow.Show();
        }
    }
}