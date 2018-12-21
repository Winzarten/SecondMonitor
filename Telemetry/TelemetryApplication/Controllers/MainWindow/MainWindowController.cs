namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using LapPicker;
    using Settings;
    using TelemetryLoad;

    public class MainWindowController : IMainWindowController
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly ITelemetryLoadController _telemetryLoadController;
        private readonly ILapPickerController _lapPickerController;

        public MainWindowController(ISettingsProvider settingsProvider, ITelemetryLoadController telemetryLoadController, ILapPickerController lapPickerController)
        {
            _settingsProvider = settingsProvider;
            _telemetryLoadController = telemetryLoadController;
            _lapPickerController = lapPickerController;
        }

        public Window MainWindow { get; set; }

        public async Task LoadTelemetrySession(string telemetryIdentifier)
        {
            await _telemetryLoadController.LoadSessionAsync(telemetryIdentifier);
        }

        public void StartController()
        {
            MainWindow.Closed+=MainWindowOnClosed;
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
        }

        private void StopChildControllers()
        {
            _lapPickerController.StopController();
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