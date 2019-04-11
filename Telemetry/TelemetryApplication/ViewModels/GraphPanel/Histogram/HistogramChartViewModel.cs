namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Histogram
{
    using System.Linq;
    using AggregatedCharts.Histogram;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using SecondMonitor.ViewModels;

    public class HistogramChartViewModel : AbstractViewModel<Histogram>
    {
        private double _bandSize;
        private PlotModel _plotModel;
        private ColumnSeries _columnSeries;

        public double BandSize
        {
            get => _bandSize;
            protected set => SetProperty(ref _bandSize, value);
        }

        public PlotModel PlotModel
        {
            get => _plotModel;
            protected set => SetProperty(ref _plotModel, value);
        }

        private void BuildPlotModel()
        {
            PlotModel model = new PlotModel
            {
                Title = OriginalModel.Title,
                IsLegendVisible = false,
                TextColor = OxyColors.White
            };

            _columnSeries = new ColumnSeries() { Title = "Percentage", StrokeColor = OxyColors.White, StrokeThickness = 1, LabelPlacement = LabelPlacement.Inside, LabelFormatString = "{0:.00}%" };
            _columnSeries.Items.AddRange(OriginalModel.Items.Select(x => new ColumnItem() { Value = x.Percentage }));

            CategoryAxis categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom, GapWidth = 0, MajorGridlineThickness = 1, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = OxyColors.White, TicklineColor = OxyColors.White, Unit = OriginalModel.Unit};
            categoryAxis.Labels.AddRange(OriginalModel.Items.Select(x => x.Category));
            LinearAxis valueAxis = new LinearAxis { Position = AxisPosition.Left, MinimumPadding = 0, MaximumPadding = 0.0, AbsoluteMinimum = 0, MajorGridlineThickness = 1, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = OxyColors.White, MajorStep = 5, AxislineColor = OxyColors.White, TicklineColor = OxyColors.White };

            model.Series.Add(_columnSeries);
            model.Axes.Add(categoryAxis);
            model.Axes.Add(valueAxis);

            PlotModel = model;
        }

        protected override void ApplyModel(Histogram model)
        {
            BandSize = model.BandSize;
            BuildPlotModel();
        }

        public override Histogram SaveToNewModel()
        {
            throw new System.NotImplementedException();
        }
    }
}