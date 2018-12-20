namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers
{
    using System.Windows;
    using ViewModels.Controllers;

    public interface IMainWindowController : IController
    {
        Window MainWindow { get; set; }
    }
}