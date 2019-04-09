namespace SecondMonitor.Telemetry.TelemetryApplication.Histogram
{
    using DataModel.Telemetry;

    public class HistogramItem
    {
        public TimedTelemetrySnapshot[] TimedTelemetrySnapshots { get; set; }

        public string Category { get; set; }

        public double Percentage { get; set; }

    }
}