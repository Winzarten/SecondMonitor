namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Chassis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.Snapshot.Systems;
    using DataModel.Telemetry;
    using OxyPlot;
    using OxyPlot.Series;
    using TelemetryManagement.DTO;

    public abstract class AbstractChassisGraphViewModel : AbstractGraphViewModel
    {
        private const string FrontLabel = "Front";
        private const string RearLabel = "Rear";
        private bool _frontVisible;
        private bool _rearVisible;

        protected AbstractChassisGraphViewModel()
        {
            _frontVisible = true;
            _rearVisible = true;
        }

        public abstract override string Title { get; }

        public bool FrontVisible
        {
            get => _frontVisible;
            set
            {
                SetProperty(ref _frontVisible, value);
                UpdateSideVisibility();
            }
        }

        public bool RearVisible
        {
            get => _rearVisible;
            set
            {
                SetProperty(ref _rearVisible, value);
                UpdateSideVisibility();
            }
        }

        protected abstract override string YUnits { get; }

        protected abstract override double YTickInterval { get; }

        protected override bool CanYZoom => true;

        protected abstract Func<CarInfo, double> FrontFunc { get; }

        protected abstract Func<CarInfo, double> RearFunc { get; }

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {

        }

        protected override List<LineSeries> GetLineSeries(LapSummaryDto lapSummary, TimedTelemetrySnapshot[] dataPoints, OxyColor color)
        {
            LineSeries[] lineSeries = new LineSeries[2];
            string baseTitle = $"Lap {lapSummary.CustomDisplayName}";

            List<DataPoint> plotDataPoints = dataPoints.Select(x => new DataPoint(GetXValue(x), FrontFunc(x.PlayerData.CarInfo))).ToList();
            double newMax = plotDataPoints.Max(x => x.Y);
            lineSeries[0] = CreateLineSeries(baseTitle + $" {FrontLabel}", color);
            lineSeries[0].IsVisible = FrontVisible;
            lineSeries[0].Points.AddRange(plotDataPoints);

            plotDataPoints = dataPoints.Select(x => new DataPoint(GetXValue(x), RearFunc(x.PlayerData.CarInfo))).ToList();
            newMax = plotDataPoints.Max(x => x.Y);
            lineSeries[1] = CreateLineSeries(baseTitle + $" {RearLabel}", color);
            lineSeries[1].IsVisible = RearVisible;
            lineSeries[1].Points.AddRange(plotDataPoints);

            if (newMax > YMaximum)
            {
                YMaximum = newMax;
            }

            return lineSeries.ToList();
        }

        private void UpdateSideVisibility()
        {
            foreach (List<LineSeries> lineSeries in LoadedSeries.Values.Select(x=>x.lineSeries))
            {
                foreach (LineSeries series in lineSeries)
                {
                    if (series.Title.Contains(FrontLabel))
                    {
                        series.IsVisible = FrontVisible;
                    }

                    if (series.Title.Contains(RearLabel))
                    {
                        series.IsVisible = RearVisible;
                    }
                }
            }
            InvalidatePlot();
        }
    }
}