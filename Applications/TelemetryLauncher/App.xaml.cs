namespace SecondMonitor.TelemetryLauncher
{
    using System.Windows;
    using Telemetry.TelemetryApplication.Controllers;
    using TelemetryPresentation.MainWindow;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            TelemetryApplicationController telemetryApplicationController = new TelemetryApplicationController(new MainWindow());
            telemetryApplicationController.StartController();
            telemetryApplicationController.OpenFromRepository("18-12-23-03-59-Norisring-Practice");
        }
    }
}
