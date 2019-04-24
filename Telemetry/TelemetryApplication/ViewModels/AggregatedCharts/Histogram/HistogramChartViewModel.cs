namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.AggregatedCharts.Histogram
{
    using System.Collections.Generic;
    using System.Windows;
    using AggregatedCharts;
    using Controllers.Synchronization;
    using DataModel.Extensions;
    using OxyPlot;
    using OxyPlot.Annotations;
    using OxyPlot.Axes;
    using SecondMonitor.ViewModels;
    using TelemetryApplication.AggregatedCharts.Histogram;
    using LinearAxis = OxyPlot.Axes.LinearAxis;
    using LinearBarSeries = OxyPlot.Series.LinearBarSeries;

    public class HistogramChartViewModel : AbstractViewModel<Histogram>, IAggregatedChartViewModel
    {
        private readonly IDataPointSelectionSynchronization _dataPointSelectionSynchronization;
        private static readonly OxyColor BaseColor = OxyColors.White;
        private static readonly OxyColor SelectedColor = OxyColors.MediumVioletRed;


        private double _bandSize;
        private PlotModel _plotModel;
        private LinearBarSeries _columnSeries;
        private int _dataPointsCount;
        private readonly Dictionary<DataPoint, HistogramBand> _pointBandMap;
        private readonly Dictionary<HistogramBand, RectangleAnnotation> _selectionAnnotations;
        private string _title;

        public HistogramChartViewModel(IDataPointSelectionSynchronization dataPointSelectionSynchronization)
        {
            _dataPointSelectionSynchronization = dataPointSelectionSynchronization;
            _pointBandMap = new Dictionary<DataPoint, HistogramBand>();
            _selectionAnnotations = new Dictionary<HistogramBand, RectangleAnnotation>();
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

        public void ToggleSelection(Point mousePoint)
        {
            if (!TryGetBandByMousePoint(mousePoint, out HistogramBand histogramBand))
            {
                return;
            }
            if (_selectionAnnotations.TryGetValue(histogramBand, out RectangleAnnotation annotation))
            {
                PlotModel.Annotations.Remove(annotation);
                _selectionAnnotations.Remove(histogramBand);
                _dataPointSelectionSynchronization.DeSelectPoints(histogramBand.SourceValues);

            }
            else
            {
                annotation = new RectangleAnnotation { MinimumX = histogramBand.Category - BandSize / 2, MaximumX = histogramBand.Category + BandSize / 2, MinimumY = 0, MaximumY = histogramBand.Percentage, ToolTip = "Bar is Selected", Fill = OxyColor.FromAColor(99, OxyColors.Blue) };
                PlotModel.Annotations.Add(annotation);
                _selectionAnnotations.Add(histogramBand, annotation);
                _dataPointSelectionSynchronization.SelectPoints(histogramBand.SourceValues);
            }

            PlotModel.InvalidatePlot(false);
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

        public void DeselectAll()
        {
            _selectionAnnotations.Keys.ForEach(x => _dataPointSelectionSynchronization.DeSelectPoints(x.SourceValues));
            _selectionAnnotations.Clear();
        }

        private bool TryGetBandByMousePoint(Point mousePoint, out HistogramBand band)
        {
            HitTestResult hitResult = _columnSeries.HitTest(new HitTestArguments(new ScreenPoint(mousePoint.X, mousePoint.Y), 0));
            if (hitResult?.Item == null || !(hitResult.Item is DataPoint dataPoint))
            {
                band = null;
                return false;
            }

            band = _pointBandMap[dataPoint];
            return true;
        }

        public void Dispose()
        {
            DeselectAll();
        }
    }
}