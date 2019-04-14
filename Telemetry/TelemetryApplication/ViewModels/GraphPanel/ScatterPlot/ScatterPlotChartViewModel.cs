namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.ScatterPlot
{
    using System.Collections.Generic;
    using System.Linq;
    using AggregatedCharts;
    using DataModel.Extensions;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using SecondMonitor.ViewModels;
    using TelemetryApplication.AggregatedCharts.ScatterPlot;

    public class ScatterPlotChartViewModel : AbstractViewModel<ScatterPlot>, IAggregatedChartViewModel
    {
        private static readonly OxyColor BaseColor = OxyColors.White;

        private PlotModel _plotModel;
        private int _dataPointsCount;
        private string _title;

        public PlotModel PlotModel
        {
            get => _plotModel;
            set => SetProperty(ref _plotModel, value);
        }

        public int DataPointsCount
        {
            get => _dataPointsCount;
            set => SetProperty(ref _dataPointsCount, value);
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
                IsLegendVisible = true,
                TextColor = BaseColor,
                PlotAreaBorderColor = BaseColor,
                LegendBorder = OxyColors.DarkRed,
                LegendBorderThickness = 1,
                LegendPlacement = LegendPlacement.Outside
            };


            LinearAxis xAxis = new LinearAxis { AxislineColor = BaseColor, Position = AxisPosition.Bottom, MajorStep = OriginalModel.XAxis.MajorTick, MinorStep = OriginalModel.XAxis.MinorTick, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = BaseColor, TicklineColor = BaseColor, Unit = OriginalModel.XAxis.Unit, ExtraGridlineStyle = LineStyle.Solid};
            LinearAxis yAxis = new LinearAxis { AxislineColor = BaseColor, Position = AxisPosition.Left, MajorStep = OriginalModel.YAxis.MajorTick, MinorStep = OriginalModel.YAxis.MinorTick, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = BaseColor, TicklineColor = BaseColor, Unit = OriginalModel.YAxis.Unit, ExtraGridlineStyle = LineStyle.Solid};
            xAxis.PositionAtZeroCrossing = true;
            yAxis.PositionAtZeroCrossing = true;
            IEnumerable<ScatterSeries> series = OriginalModel.ScatterPlotSeries.Select(BuildScatterSeries);

            series.ForEach(model.Series.Add);
            model.Axes.Add(xAxis);
            model.Axes.Add(yAxis);

            PlotModel = model;
            DataPointsCount = OriginalModel.ScatterPlotSeries.Select(x => x.DataPoints.Count).Sum();
        }

        protected ScatterSeries BuildScatterSeries(ScatterPlotSeries scatterPlotSeries)
        {
            ScatterSeries scatterSeries = new ScatterSeries(){Title = scatterPlotSeries.SeriesName, MarkerFill = scatterPlotSeries.Color, MarkerType = MarkerType.Circle, MarkerSize = 3, TrackerFormatString = "{0}\n" + OriginalModel.XAxis.Unit + ": {2}\n" + OriginalModel.YAxis.Unit + ": {4}" };
            scatterSeries.Points.AddRange(scatterPlotSeries.DataPoints.Select(x => new ScatterPoint(x.X, x.Y)));
            return scatterSeries;
        }

        protected override void ApplyModel(ScatterPlot model)
        {
           BuildPlotModel();
        }

        public override ScatterPlot SaveToNewModel()
        {
            throw new System.NotImplementedException();
        }
    }
}