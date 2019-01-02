namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LapPicker
{
    using System;
    using System.Windows.Media;
    using Controllers.Synchronization;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public interface ILapSummaryViewModel : IViewModel<LapSummaryDto>
    {
        ILapColorSynchronization LapColorSynchronization { get; set; }
        int LapNumber { get; }
        TimeSpan LapTime { get; }
        bool Selected { get; set; }
        Color LapColor { get; set; }
        SolidColorBrush LapColorBrush { get; }

    }
}