namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot
{
    using System.Collections.Generic;
    using WindowsControls.Properties;
    using OxyPlot.Annotations;

    public class ScatterPlot
    {
        private readonly List<ScatterPlotSeries> _scatterPlotSeries;
        private readonly List<Annotation> _annotations;

        public ScatterPlot(string title, AxisDefinition xAxis, AxisDefinition yAxis)
        {
            Title = title;
            XAxis = xAxis;
            YAxis = yAxis;
            _scatterPlotSeries = new List<ScatterPlotSeries>();
            _annotations = new List<Annotation>();
        }

        public string Title { get; }
        public AxisDefinition XAxis { get; }
        public AxisDefinition YAxis { get; }
        public IReadOnlyCollection<Annotation> Annotations => _annotations.AsReadOnly();

        public IReadOnlyCollection<ScatterPlotSeries> ScatterPlotSeries => _scatterPlotSeries.AsReadOnly();

        public void AddScatterPlotSeries([CanBeNull]ScatterPlotSeries newSeries)
        {
            if (newSeries == null)
            {
                return;
            }

            _scatterPlotSeries.Add(newSeries);
        }

        public void AddAnnotation(Annotation annotation)
        {
            _annotations.Add(annotation);
        }

    }
}