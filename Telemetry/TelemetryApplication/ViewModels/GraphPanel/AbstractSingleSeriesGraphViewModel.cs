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
            LineSeries newLineSeries = CreateLineSeries($"Lap {lapSummary.LapNumber}", color);
            List<DataPoint> plotDataPoints = dataPoints.Select(x => new DataPoint(GetXValue(x), GetYValue(x))).ToList();

            newLineSeries.Points.AddRange(plotDataPoints);

            List<LineSeries> series = new List<LineSeries>(1) {newLineSeries};
            return series;
        }

        protected abstract double GetYValue(TimedTelemetrySnapshot value);
    }


}