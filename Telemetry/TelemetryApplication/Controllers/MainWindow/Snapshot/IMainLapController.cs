namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.Snapshot
{
    using SecondMonitor.ViewModels.Controllers;
    using TelemetryManagement.DTO;

    public interface IMainLapController : IController
    {
        LapSummaryDto MainLap { get; set; }
    }
}