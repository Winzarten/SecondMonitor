namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.Snapshot
{
    using ViewModels.SnapshotSection;

    public interface IRequireSnapshotViewModel
    {
        ISnapshotSectionViewModel SnapshotSectionViewModel { get; set; }
    }
}