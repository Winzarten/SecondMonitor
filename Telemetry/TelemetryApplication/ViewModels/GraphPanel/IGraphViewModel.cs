namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System;
    using Controllers.Synchronization;
    using LiveCharts;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public interface IGraphViewModel : IViewModel
    {
        SeriesCollection SeriesCollection { get; }
        ILapColorSynchronization LapColorSynchronization { get; set; }

        void AddLapTelemetry(LapTelemetryDto lapTelemetryDto);
        void RemoveLapTelemetry(LapSummaryDto lapSummaryDto);

        Func<double, string> XFormatter { get; }
        Func<double, string> YFormatter { get; }
    }
}