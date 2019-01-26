namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.Snapshot
{
    using SecondMonitor.ViewModels.Controllers;
    using ViewModels;

    public interface ISnapshotSectionController : IController
    {
        IMainWindowViewModel MainWindowViewModel { get; set; }
    }
}