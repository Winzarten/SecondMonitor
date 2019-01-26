namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Controllers.Synchronization.Graphs;
    using DataModel.Snapshot.Systems;
    using DataModel.Telemetry;
    using OxyPlot;
    using OxyPlot.Series;
    using TelemetryManagement.DTO;

    public abstract class AbstractWheelsGraphViewModel : AbstractGraphViewModel
    {
        private const string FrontLeftName = "Front L";
        private const string FrontRightName = "Front R";
        private const string RearLeftName = "Rear L";
        private const string RearRightName = "Rear R";

        private bool _frontLeftVisible;
        private bool _frontRightVisible;
        private bool _rearLeftVisible;
        private bool _rearRightVisible;

        protected AbstractWheelsGraphViewModel()
        {
            _frontLeftVisible = true;
            _frontRightVisible = true;
            _rearRightVisible = true;
            _rearLeftVisible = true;
        }

        public bool FrontLeftVisible
        {
            get => _frontLeftVisible;
            set
            {
                SetProperty(ref _frontLeftVisible, value);
                UpdateWheelsVisibility();
                NotifyGraphsOfWheelVisibilityChange();
            }
        }

        public bool FrontRightVisible
        {
            get => _frontRightVisible;
            set
            {
                SetProperty(ref _frontRightVisible, value);
                UpdateWheelsVisibility();
                NotifyGraphsOfWheelVisibilityChange();
            }
        }

        public bool RearLeftVisible
        {
            get => _rearLeftVisible;
            set
            {
                SetProperty(ref _rearLeftVisible, value);
                UpdateWheelsVisibility();
                NotifyGraphsOfWheelVisibilityChange();
            }
        }

        public bool RearRightVisible
        {
            get => _rearRightVisible;
            set
            {
                SetProperty(ref _rearRightVisible, value);
                UpdateWheelsVisibility();
                NotifyGraphsOfWheelVisibilityChange();
            }
        }

        protected abstract Func<WheelInfo, double> ExtractorFunction { get; }


        protected virtual LineStyle[] LineStyles => new[] { LineStyle.Solid, LineStyle.Solid, LineStyle.Solid, LineStyle.Solid };



        protected override List<LineSeries> GetLineSeries(LapSummaryDto lapSummary, TimedTelemetrySnapshot[] dataPoints, OxyColor color)
        {
            LineSeries[] lineSeries = new LineSeries[4];
            string baseTitle = $"Lap {lapSummary.LapNumber}";


            List<DataPoint> plotDataPoints = dataPoints.Select(x => new DataPoint(GetXValue(x), ExtractorFunction(x.PlayerData.CarInfo.WheelsInfo.FrontLeft))).ToList();
            double newMax = plotDataPoints.Max(x => x.Y);
            double newMin = plotDataPoints.Min(x => x.Y);
            lineSeries[0] = CreateLineSeries(baseTitle + $" {FrontLeftName}", color, LineStyles[0]);
            lineSeries[0].IsVisible = FrontLeftVisible;
            lineSeries[0].Points.AddRange(plotDataPoints);

            plotDataPoints = dataPoints.Select(x => new DataPoint(GetXValue(x), ExtractorFunction(x.PlayerData.CarInfo.WheelsInfo.FrontRight))).ToList();
            lineSeries[1] = CreateLineSeries(baseTitle + $" {FrontRightName}", color, LineStyles[1]);
            lineSeries[1].Points.AddRange(plotDataPoints);
            lineSeries[1].IsVisible = FrontRightVisible;
            newMax = Math.Max(newMax, plotDataPoints.Max(x => x.Y));
            newMin = Math.Min(newMin, plotDataPoints.Min(x => x.Y));

            plotDataPoints = dataPoints.Select(x => new DataPoint(GetXValue(x), ExtractorFunction(x.PlayerData.CarInfo.WheelsInfo.RearLeft))).ToList();
            lineSeries[2] = CreateLineSeries(baseTitle + $" {RearLeftName}", color, LineStyles[2]);
            lineSeries[2].Points.AddRange(plotDataPoints);
            lineSeries[2].IsVisible = RearLeftVisible;
            newMax = Math.Max(newMax, plotDataPoints.Max(x => x.Y));
            newMin = Math.Min(newMin, plotDataPoints.Min(x => x.Y));

            plotDataPoints = dataPoints.Select(x => new DataPoint(GetXValue(x), ExtractorFunction(x.PlayerData.CarInfo.WheelsInfo.RearRight))).ToList();
            lineSeries[3] = CreateLineSeries(baseTitle + $" {RearRightName}", color, LineStyles[3]);
            lineSeries[3].Points.AddRange(plotDataPoints);
            lineSeries[3].IsVisible = RearRightVisible;
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

        protected override void UnsubscribeGraphViewSync()
        {
            base.UnsubscribeGraphViewSync();

            if (GraphViewSynchronization == null)
            {
                return;
            }

            GraphViewSynchronization.WheelVisibilityChanged -= GraphViewSynchronizationOnWheelVisibilityChanged;
        }

        protected override void SubscribeGraphViewSync()
        {
            base.SubscribeGraphViewSync();

            if (GraphViewSynchronization == null)
            {
                return;
            }

            GraphViewSynchronization.WheelVisibilityChanged += GraphViewSynchronizationOnWheelVisibilityChanged;
        }

        private void NotifyGraphsOfWheelVisibilityChange()
        {
            if (!SyncWithOtherGraphs)
            {
                return;
            }

            GraphViewSynchronization.NotifyWheelVisibilityChanged(this, FrontLeftVisible, FrontRightVisible, RearLeftVisible, RearRightVisible);
        }

        private void GraphViewSynchronizationOnWheelVisibilityChanged(object sender, WheelVisibilityArgs e)
        {
            if (ReferenceEquals(sender, this) || !SyncWithOtherGraphs)
            {
                return;
            }

            _frontLeftVisible = e.FrontLeftVisible;
            _frontRightVisible = e.FrontRightVisible;
            _rearLeftVisible = e.RearLeftVisible;
            _rearRightVisible = e.RearRightVisible;

            UpdateWheelsVisibility();

            NotifyPropertyChanged(nameof(FrontLeftVisible));
            NotifyPropertyChanged(nameof(FrontRightVisible));
            NotifyPropertyChanged(nameof(RearLeftVisible));
            NotifyPropertyChanged(nameof(RearRightVisible));
        }

        private void UpdateWheelsVisibility()
        {
            foreach (List<LineSeries> lineSeries in LoadedSeries.Values)
            {
                foreach (LineSeries series in lineSeries)
                {
                    if (series.Title.Contains(FrontLeftName))
                    {
                        series.IsVisible = FrontLeftVisible;
                    }

                    if (series.Title.Contains(FrontRightName))
                    {
                        series.IsVisible = FrontRightVisible;
                    }

                    if (series.Title.Contains(RearLeftName))
                    {
                        series.IsVisible = RearLeftVisible;
                    }

                    if (series.Title.Contains(RearRightName))
                    {
                        series.IsVisible = RearRightVisible;
                    }
                }
            }
            InvalidatePlot();
        }

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {

        }
    }
}