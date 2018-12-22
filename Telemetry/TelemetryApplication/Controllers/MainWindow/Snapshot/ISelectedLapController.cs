namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.Snapshot
{
    using TelemetryManagement.DTO;

    public interface ISelectedLapController
    {
        LapSummaryDto SelectedLapSummaryDto { get; set; }
    }
}