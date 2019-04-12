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
        private static readonly OxyColor BaseColor = OxyColors.White;

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
                TextColor = BaseColor,
                PlotAreaBorderColor = BaseColor,
            };

            _columnSeries = new ColumnSeries() { Title = "Percentage", StrokeColor = BaseColor, StrokeThickness = 1, LabelPlacement = LabelPlacement.Inside, LabelFormatString = "{0:.00}" };
            _columnSeries.Items.AddRange(OriginalModel.Items.Select(x => new ColumnItem() { Value = x.Percentage }));


            CategoryAxis categoryAxis = new CategoryAxis { IsTickCentered = true, AxislineColor = BaseColor,  Position = AxisPosition.Bottom, GapWidth = 0, MajorStep = 5, MinorStep = 1, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = BaseColor, MinorGridlineStyle = LineStyle.Solid, MinorGridlineColor = BaseColor, TicklineColor = BaseColor, Unit = OriginalModel.Unit, ExtraGridlineStyle = LineStyle.Solid, ExtraGridlineColor = OxyColors.Red};
            categoryAxis.Labels.AddRange(OriginalModel.Items.Select(x => x.Category));
            int zeroSeriesIndex = categoryAxis.Labels.IndexOf("0");
            LinearAxis valueAxis = new LinearAxis { Position = AxisPosition.Left, MinimumPadding = 0, MaximumPadding = 0.0, AbsoluteMinimum = 0, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = BaseColor, MajorStep = 5, AxislineColor = BaseColor, TicklineColor = BaseColor, MinorGridlineStyle = LineStyle.Dot, MinorGridlineColor = BaseColor};

            model.Series.Add(_columnSeries);
            model.Axes.Add(categoryAxis);
            model.Axes.Add(valueAxis);

            if (zeroSeriesIndex >= 0)
            {
                _columnSeries.Items[zeroSeriesIndex].Color = OxyColors.Red;
                categoryAxis.ExtraGridlines = new double[] { zeroSeriesIndex };
            }

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