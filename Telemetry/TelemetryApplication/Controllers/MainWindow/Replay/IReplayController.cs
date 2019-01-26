namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.Replay
{
    using Snapshot;
    using ViewModels.SnapshotSection;

    public interface IReplayController : IMainLapController
    {
        ISnapshotSectionViewModel SnapshotSectionViewModel { set; }
    }
}