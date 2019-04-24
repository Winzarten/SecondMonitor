namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.AggregatedCharts
{
    using System;
    using SecondMonitor.ViewModels;

    public interface IAggregatedChartViewModel : IViewModel, IDisposable
    {
        string Title { get; }
    }
}