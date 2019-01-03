namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.Telemetry;
    using OxyPlot;
    using OxyPlot.Series;
    using TelemetryManagement.DTO;

    public abstract class AbstractSingleSeriesGraphViewModel : AbstractGraphViewModel
    {
        protected override List<LineSeries> GetLineSeries(LapSummaryDto lapSummary, TimedTelemetrySnapshot[] dataPoints, OxyColor color)
        {
            LineSeries newLineSeries = new LineSeries
            {
                Title = $"Lap {lapSummary.LapNumber}",
                Color = color,
                TextColor = color,
                InterpolationAlgorithm = null,
                CanTrackerInterpolatePoints = false,
                StrokeThickness = 2
            };

            List<DataPoint> plotDataPoints = dataPoints.Select(x => new DataPoint(GetXValue(x), GetYValue(x))).ToList();

            newLineSeries.Points.AddRange(plotDataPoints);

            List<LineSeries> series = new List<LineSeries>(1) {newLineSeries};
            return series;
        }
    }
}