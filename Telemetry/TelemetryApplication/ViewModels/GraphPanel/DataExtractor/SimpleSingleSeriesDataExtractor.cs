namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.DataExtractor
{
    using System;
    using DataModel.Telemetry;
    using Settings.DTO;
    using TelemetryManagement.DTO;

    public class SimpleSingleSeriesDataExtractor : ISingleSeriesDataExtractor
    {
        public event EventHandler<EventArgs> DataRefreshRequested;

        public string ExtractorName => "Normal";

        public double GetValue(Func<TimedTelemetrySnapshot, double> valueExtractFunction, TimedTelemetrySnapshot telemetrySnapshot, XAxisKind xAxisKind)
        {
            return valueExtractFunction(telemetrySnapshot);
        }

        public bool ShowLapGraph(LapSummaryDto lapSummaryDto)
        {
            return true;
        }
    }
}