namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Histogram
{
    using System.Collections.Generic;
    using AggregatedCharts;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using SecondMonitor.ViewModels;
    using TelemetryApplication.AggregatedCharts.Histogram;

    public class HistogramChartViewModel : AbstractViewModel<Histogram>, IAggregatedChartViewModel
    {
        private static readonly OxyColor BaseColor = OxyColors.White;
        private static readonly OxyColor SelectedColor = OxyColors.MediumVioletRed;


        private double _bandSize;
        private PlotModel _plotModel;
        private LinearBarSeries _columnSeries;
        private int _dataPointsCount;
        private readonly Dictionary<DataPoint, HistogramBand> _pointBandMap;
        private string _title;

        public HistogramChartViewModel()
        {
            _pointBandMap = new Dictionary<DataPoint, HistogramBand>();
        }

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

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private void BuildPlotModel()
        {
            PlotModel model = new PlotModel
            {
                Title = OriginalModel.Title,
                IsLegendVisible = false,
                TextColor = BaseColor,
                PlotAreaBorderColor = BaseColor,
                SelectionColor = SelectedColor,
            };

            _columnSeries = new LinearBarSeries() {TrackerFormatString = OriginalModel.Unit+ ": {2:0.00}\n%: {4:0.00}", Title = "Percentage", StrokeColor = BaseColor, StrokeThickness = 1, BarWidth = double.MaxValue, SelectionMode = SelectionMode.Multiple, Selectable = true};
            foreach (HistogramBand histogramBand in OriginalModel.Items)
            {
                DataPoint newPoint = new DataPoint(histogramBand.Category, histogramBand.Percentage);
                _pointBandMap.Add(newPoint, histogramBand);
                _columnSeries.Points.Add(newPoint);
            }
            //_columnSeries.Points.AddRange(OriginalModel.Items.Select( x=> new DataPoint(x.Category, x.Percentage)));


            LinearAxis barAxis = new LinearAxis {MinimumMinorStep = BandSize, AxislineColor = BaseColor, Position = AxisPosition.Bottom, MajorStep = BandSize * 5, MinorStep = BandSize, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = BaseColor, TicklineColor = BaseColor, Unit = OriginalModel.Unit, ExtraGridlineStyle = LineStyle.Solid, ExtraGridlineColor = OxyColors.Red, ExtraGridlineThickness = 2, ExtraGridlines = new double[] { 0}, Selectable = true};
            LinearAxis valueAxis = new LinearAxis {Unit = "%", Position = AxisPosition.Left, MinimumPadding = 0, MaximumPadding = 0.0, AbsoluteMinimum = 0, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = BaseColor, MajorStep = 5, AxislineColor = BaseColor, TicklineColor = BaseColor, MinorGridlineStyle = LineStyle.Dot, MinorGridlineColor = BaseColor, Selectable = true};

            barAxis.PositionAtZeroCrossing = true;
            model.Series.Add(_columnSeries);
            model.Axes.Add(barAxis);
            model.Axes.Add(valueAxis);
            PlotModel = model;
            DataPointsCount = OriginalModel.DataPointsCount;

        }

        protected override void ApplyModel(Histogram model)
        {
            _pointBandMap.Clear();;
            BandSize = model.BandSize;
            BuildPlotModel();
        }

        public override Histogram SaveToNewModel()
        {
            throw new System.NotImplementedException();
        }
    }
}