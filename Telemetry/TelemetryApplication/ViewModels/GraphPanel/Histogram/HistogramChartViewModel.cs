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
        private LinearBarSeries _columnSeries;
        private int _dataPointsCount;

        public int DataPointsCount
        {
            get => _dataPointsCount;
            set => SetProperty(ref _dataPointsCount, value);
        }

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

            _columnSeries = new LinearBarSeries() {TrackerFormatString = OriginalModel.Unit+ ": {2:0.00}\n%: {4:0.00}", Title = "Percentage", StrokeColor = BaseColor, StrokeThickness = 1, BarWidth = double.MaxValue};
            _columnSeries.Points.AddRange(OriginalModel.Items.Select( x=> new DataPoint(x.Category, x.Percentage)));


            LinearAxis barAxis = new LinearAxis {MinimumMinorStep = BandSize, AxislineColor = BaseColor, Position = AxisPosition.Bottom, MajorStep = BandSize * 5, MinorStep = BandSize, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = BaseColor, TicklineColor = BaseColor, Unit = OriginalModel.Unit, ExtraGridlineStyle = LineStyle.Solid, ExtraGridlineColor = OxyColors.Red, ExtraGridlineThickness = 2, ExtraGridlines = new double[] { 0}};
            LinearAxis valueAxis = new LinearAxis {Unit  = "%", Position = AxisPosition.Left, MinimumPadding = 0, MaximumPadding = 0.0, AbsoluteMinimum = 0, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = BaseColor, MajorStep = 5, AxislineColor = BaseColor, TicklineColor = BaseColor, MinorGridlineStyle = LineStyle.Dot, MinorGridlineColor = BaseColor};

            barAxis.PositionAtZeroCrossing = true;
            model.Series.Add(_columnSeries);
            model.Axes.Add(barAxis);
            model.Axes.Add(valueAxis);

            PlotModel = model;
            DataPointsCount = OriginalModel.DataPointsCount;
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