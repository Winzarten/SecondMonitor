namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot
{
    using System.Collections.Generic;
    using WindowsControls.Properties;

    public class ScatterPlot
    {
        private readonly List<ScatterPlotSeries> _scatterPlotSeries;

        public ScatterPlot(string title, AxisDefinition xAxis, AxisDefinition yAxis)
        {
            Title = title;
            XAxis = xAxis;
            YAxis = yAxis;
            _scatterPlotSeries = new List<ScatterPlotSeries>();
        }

        public string Title { get; }
        public AxisDefinition XAxis { get; }
        public AxisDefinition YAxis { get; }

        public IReadOnlyCollection<ScatterPlotSeries> ScatterPlotSeries => _scatterPlotSeries.AsReadOnly();

        public void AddScatterPlotSeries([CanBeNull]ScatterPlotSeries newSeries)
        {
            if (newSeries == null)
            {
                return;
            }

            _scatterPlotSeries.Add(newSeries);
        }

    }
}