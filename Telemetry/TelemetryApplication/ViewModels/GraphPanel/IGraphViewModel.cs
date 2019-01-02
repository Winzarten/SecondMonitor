namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System;
    using LiveCharts;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public interface IGraphViewModel : IViewModel<LapTelemetryDto>
    {
        SeriesCollection SeriesCollection { get; }

        Func<double, string> XFormatter { get; }
        Func<double, string> YFormatter { get; }
    }
}