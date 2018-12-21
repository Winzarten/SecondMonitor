namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow
{
    using System.Threading.Tasks;
    using System.Windows;
    using ViewModels.Controllers;

    public interface IMainWindowController : IController
    {

        Window MainWindow { get; set; }

        Task LoadTelemetrySession(string telemetryIdentifier);
    }
}