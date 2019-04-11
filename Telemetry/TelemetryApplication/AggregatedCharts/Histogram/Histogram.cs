namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram
{
    using System.Collections.Generic;

    public class Histogram
    {
        private readonly List<HistogramBand> _histogramItems;

        public Histogram()
        {
            _histogramItems = new List<HistogramBand>();
        }

        public string Title { get; set; }
        public IReadOnlyCollection<HistogramBand> Items => _histogramItems.AsReadOnly();
        public double BandSize { get; set; }
        public string Unit { get; set; }

        public void AddItem(HistogramBand histogramBand)
        {
            _histogramItems.Add(histogramBand);
        }

        public void AddItems(IEnumerable<HistogramBand> histogramItems)
        {
            _histogramItems.AddRange(histogramItems);
        }
    }
}