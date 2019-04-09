namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Histogram
{
    using System.Linq;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using SecondMonitor.ViewModels;
    using TelemetryApplication.Histogram;

    public class HistogramChartViewModel : AbstractViewModel
    {
        private double _bandSize;
        private PlotModel _plotModel;
        private ColumnSeries _columnSeries;
        private Histogram _histogram;

        public HistogramChartViewModel()
        {
            BuildPlotModel();
        }

        public Histogram Histogram
        {
            get => Histogram;
            set
            {
                SetProperty(ref _histogram, value);
                BandSize = _histogram.BandSize;
                BuildPlotModel();
            }
        }

        public double BandSize
        {
            get => _bandSize;
            set => SetProperty(ref _bandSize, value);
        }

        public PlotModel PlotModel
        {
            get => _plotModel;
            set => SetProperty(ref _plotModel, value);
        }

        private void BuildPlotModel()
        {
            PlotModel model = new PlotModel
            {
                Title = Histogram.Title,
                IsLegendVisible = false,
                TextColor = OxyColors.White
            };

            _columnSeries = new ColumnSeries() { Title = "Percentage", StrokeColor = OxyColors.White, StrokeThickness = 1, LabelPlacement = LabelPlacement.Inside, LabelFormatString = "{0:.00}%" };
            _columnSeries.Items.AddRange(Histogram.Items.Select(x => new ColumnItem() { Value = x.Percentage }));

            CategoryAxis categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom, GapWidth = 0, MajorGridlineThickness = 1, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = OxyColors.White, TicklineColor = OxyColors.White };
            categoryAxis.Labels.AddRange(Histogram.Items.Select(x => x.Category));
            LinearAxis valueAxis = new LinearAxis { Position = AxisPosition.Left, MinimumPadding = 0, MaximumPadding = 0.0, AbsoluteMinimum = 0, MajorGridlineThickness = 1, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = OxyColors.White, MajorStep = 10, AxislineColor = OxyColors.White, TicklineColor = OxyColors.White };

            model.Series.Add(_columnSeries);
            model.Axes.Add(categoryAxis);
            model.Axes.Add(valueAxis);

            PlotModel = model;
        }

    }
}