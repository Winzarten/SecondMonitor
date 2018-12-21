namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LapPicker
{
    using System;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public interface ILapSummaryViewModel : IAbstractViewModel<LapSummaryDto>
    {
        int LapNumber { get; }
        TimeSpan LapTime { get; }
        bool Display { get; set; }
    }
}