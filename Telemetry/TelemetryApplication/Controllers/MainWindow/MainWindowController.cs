namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow
{
    using System;
    using System.Windows;
    using Settings;

    public class MainWindowController : IMainWindowController
    {
        private readonly ISettingsProvider _settingsProvider;

        public MainWindowController(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public Window MainWindow { get; set; }

        public void StartController()
        {
            MainWindow.Closed+=MainWindowOnClosed;
            ShowMainWindow();
        }

        private void MainWindowOnClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShowMainWindow()
        {
            MainWindow.Show();
        }
    }
}