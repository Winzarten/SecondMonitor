namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.GraphPanel
{
    using SecondMonitor.ViewModels.Controllers;

    public interface IGraphPanelController : IController
    {
        bool IsLetPanel { get; }
    }
}