namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram
{
    using System.Collections.Generic;

    public class Histogram
    {
        private readonly List<HistogramItem> _histogramItems;

        public Histogram()
        {
            _histogramItems = new List<HistogramItem>();
        }

        public string Title { get; set; }
        public IReadOnlyCollection<HistogramItem> Items => _histogramItems.AsReadOnly();
        public double BandSize { get; set; }

        public void AddItem(HistogramItem histogramItem)
        {
            _histogramItems.Add(histogramItem);
        }

        public void AddItems(IEnumerable<HistogramItem> histogramItems)
        {
            _histogramItems.AddRange(histogramItems);
        }
    }
}