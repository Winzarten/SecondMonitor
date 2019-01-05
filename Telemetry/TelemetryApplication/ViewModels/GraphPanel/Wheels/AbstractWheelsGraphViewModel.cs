namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.Snapshot.Systems;
    using DataModel.Telemetry;
    using OxyPlot;
    using OxyPlot.Series;
    using TelemetryManagement.DTO;

    public abstract class AbstractWheelsGraphViewModel : AbstractGraphViewModel
    {
        protected abstract Func<WheelInfo, double> ExtractorFunction { get; }

        //protected virtual LineStyle[] LineStyles => new[] {LineStyle.Solid, LineStyle.Dot, LineStyle.Dash, LineStyle.DashDot};
        protected virtual LineStyle[] LineStyles => new[] { LineStyle.Solid, LineStyle.Solid, LineStyle.Solid, LineStyle.Solid };

        protected override List<LineSeries> GetLineSeries(LapSummaryDto lapSummary, TimedTelemetrySnapshot[] dataPoints, OxyColor color)
        {
            LineSeries[] lineSeries = new LineSeries[4];
            string baseTitle = $"Lap {lapSummary.LapNumber}";


            List<DataPoint> plotDataPoints = dataPoints.Select(x => new DataPoint(GetXValue(x), ExtractorFunction(x.PlayerData.CarInfo.WheelsInfo.FrontLeft))).ToList();
            double newMax = plotDataPoints.Max(x => x.Y);
            double newMin = plotDataPoints.Min(x => x.Y);
            lineSeries[0] = CreateLineSeries(baseTitle + " Front L", color, LineStyles[0]);
            lineSeries[0].Points.AddRange(plotDataPoints);

            plotDataPoints = dataPoints.Select(x => new DataPoint(GetXValue(x), ExtractorFunction(x.PlayerData.CarInfo.WheelsInfo.FrontRight))).ToList();
            lineSeries[1] = CreateLineSeries(baseTitle + " Front R", color, LineStyles[1]);
            lineSeries[1].Points.AddRange(plotDataPoints);
            newMax = Math.Max(newMax, plotDataPoints.Max(x => x.Y));
            newMin = Math.Min(newMin, plotDataPoints.Min(x => x.Y));

            plotDataPoints = dataPoints.Select(x => new DataPoint(GetXValue(x), ExtractorFunction(x.PlayerData.CarInfo.WheelsInfo.RearLeft))).ToList();
            lineSeries[2] = CreateLineSeries(baseTitle + " Rear L", color, LineStyles[2]);
            lineSeries[2].Points.AddRange(plotDataPoints);
            newMax = Math.Max(newMax, plotDataPoints.Max(x => x.Y));
            newMin = Math.Min(newMin, plotDataPoints.Min(x => x.Y));

            plotDataPoints = dataPoints.Select(x => new DataPoint(GetXValue(x), ExtractorFunction(x.PlayerData.CarInfo.WheelsInfo.RearRight))).ToList();
            lineSeries[3] = CreateLineSeries(baseTitle + " Rear R", color, LineStyles[3]);
            lineSeries[3].Points.AddRange(plotDataPoints);
            newMax = Math.Max(newMax, plotDataPoints.Max(x => x.Y));
            newMin = Math.Min(newMin, plotDataPoints.Min(x => x.Y));

            if (newMax > YMaximum)
            {
                YMaximum = newMax;
            }

            /*if (newMin < YMinimum || YMinimum == 0)
            {
                YMinimum = newMin;
            }*/

            return lineSeries.ToList();
        }

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {

        }
    }
}