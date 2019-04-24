namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.DataExtractor
{
    using System;
    using DataModel.Telemetry;
    using Settings.DTO;
    using TelemetryManagement.DTO;

    public interface ISingleSeriesDataExtractor : IDisposable
    {
        event EventHandler<EventArgs> DataRefreshRequested;

        string ExtractorName { get; }

        double GetValue(Func<TimedTelemetrySnapshot, double> valueExtractFunction, TimedTelemetrySnapshot telemetrySnapshot, XAxisKind xAxisKind);

        bool ShowLapGraph(LapSummaryDto lapSummaryDto);
    }
}