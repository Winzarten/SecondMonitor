namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram
{
    using TelemetryManagement.StoryBoard;

    public class HistogramBand
    {
        public HistogramBand(TimedValue[] sourceValues, string category, double percentage)
        {
            SourceValues = sourceValues;
            Category = category;
            Percentage = percentage;
        }

        public TimedValue[] SourceValues { get; set; }

        public string Category { get; set; }

        public double Percentage { get; set; }


    }
}