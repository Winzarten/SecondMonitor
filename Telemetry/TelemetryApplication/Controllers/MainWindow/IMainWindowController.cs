namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using SecondMonitor.ViewModels.Controllers;

    public interface IMainWindowController : IController
    {
        Window MainWindow { get; set; }

        Task LoadTelemetrySession(string telemetryIdentifier);

        Task LoadLastTelemetrySession();
    }
}