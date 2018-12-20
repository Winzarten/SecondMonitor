namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers
{
    using System;
    using System.Windows;

    public class MainWindowController : IMainWindowController
    {

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