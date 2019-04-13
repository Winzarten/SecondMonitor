namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;
    using DataModel.Telemetry;
    using OxyPlot;
    using OxyPlot.Series;
    using TelemetryManagement.DTO;

    public abstract class AbstractTyreTemperaturesViewModel : AbstractGraphViewModel
    {
        private const string LeftTemperatureName = "Left";
        private const string RightTemperatureName = "Right";
        private const string MiddleTemperatureName = "Middle";
        private const string CoreTemperatureName = "Core";

        private bool _leftTemperatureVisible;
        private bool _middleTemperatureVisible;
        private bool _rightTemperatureVisible;
        private bool _coreTemperatureVisible;

        public override string Title => TyrePrefix + " Tyre Temperatures";
        protected abstract string TyrePrefix { get; }
        protected override string YUnits => Temperature.GetUnitSymbol(TemperatureUnits);
        protected override double YTickInterval => 20;
        protected override bool CanYZoom => true;

        private const LineStyle LeftSideLineStyle = LineStyle.Solid;
        private const LineStyle RightSideLineStyle = LineStyle.Solid;
        private const LineStyle CenterSideLineStyle = LineStyle.Solid;
        private const LineStyle CoreSideLineStyle = LineStyle.Solid;

        protected AbstractTyreTemperaturesViewModel()
        {
            _leftTemperatureVisible = true;
            _middleTemperatureVisible = true;
            _rightTemperatureVisible = true;
            _coreTemperatureVisible = true;
        }

        public bool LeftTemperatureVisible
        {
            get => _leftTemperatureVisible;
            set
            {
                SetProperty(ref _leftTemperatureVisible, value);
                NotifyTyreTempVisibilityChanged();
                UpdateSeriesVisibility();
            }
        }

        public bool MiddleTemperatureVisible
        {
            get => _middleTemperatureVisible;
            set
            {
                SetProperty(ref _middleTemperatureVisible, value);
                NotifyTyreTempVisibilityChanged();
                UpdateSeriesVisibility();
            }
        }

        public bool RightTemperatureVisible
        {
            get => _rightTemperatureVisible;
            set
            {
                SetProperty(ref _rightTemperatureVisible, value);
                NotifyTyreTempVisibilityChanged();
                UpdateSeriesVisibility();
            }
        }

        public bool CoreTemperatureVisible
        {
            get => _coreTemperatureVisible;
            set
            {
                SetProperty(ref _coreTemperatureVisible, value);
                NotifyTyreTempVisibilityChanged();
                UpdateSeriesVisibility();
            }
        }

        protected abstract Func<Wheels, WheelInfo> WheelSelectionFunction { get; }

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {

        }

        protected override void SubscribeGraphViewSync()
        {
            base.SubscribeGraphViewSync();
            if (GraphViewSynchronization == null)
            {
                return;
            }

            GraphViewSynchronization.TyreTempVisibilityChanged += GraphViewSynchronization_TyreTempVisibilityChanged;
        }

        protected override void UnsubscribeGraphViewSync()
        {
            base.UnsubscribeGraphViewSync();
            if (GraphViewSynchronization == null)
            {
                return;
            }

            GraphViewSynchronization.TyreTempVisibilityChanged -= GraphViewSynchronization_TyreTempVisibilityChanged;
        }

        protected override List<LineSeries> GetLineSeries(LapSummaryDto lapSummary, TimedTelemetrySnapshot[] dataPoints, OxyColor color)
        {
            LineSeries[] lineSeries = new LineSeries[4];

            string baseTitle = $"Lap {lapSummary.CustomDisplayName}";
            lineSeries[0] = CreateLineSeries(baseTitle + " " + LeftTemperatureName, color.ChangeSaturation(0.5), LeftSideLineStyle);
            lineSeries[0].IsVisible = LeftTemperatureVisible;

            lineSeries[1] = CreateLineSeries(baseTitle + " " + RightTemperatureName, color.ChangeSaturation(1.5), RightSideLineStyle);
            lineSeries[1].IsVisible = RightTemperatureVisible;

            lineSeries[2] = CreateLineSeries(baseTitle + " " + MiddleTemperatureName, color, CenterSideLineStyle);
            lineSeries[2].IsVisible = MiddleTemperatureVisible;

            lineSeries[3] = CreateLineSeries(baseTitle + " " + CoreTemperatureName , color.ChangeIntensity(0.5), CoreSideLineStyle);
            lineSeries[3].IsVisible = CoreTemperatureVisible;

            lineSeries[0].Points.AddRange(dataPoints.Select(x => new DataPoint(GetXValue(x), WheelSelectionFunction(x.PlayerData.CarInfo.WheelsInfo).LeftTyreTemp.ActualQuantity.GetValueInUnits(TemperatureUnits))));
            lineSeries[1].Points.AddRange(dataPoints.Select(x => new DataPoint(GetXValue(x), WheelSelectionFunction(x.PlayerData.CarInfo.WheelsInfo).RightTyreTemp.ActualQuantity.GetValueInUnits(TemperatureUnits))));
            lineSeries[2].Points.AddRange(dataPoints.Select(x => new DataPoint(GetXValue(x), WheelSelectionFunction(x.PlayerData.CarInfo.WheelsInfo).CenterTyreTemp.ActualQuantity.GetValueInUnits(TemperatureUnits))));
            lineSeries[3].Points.AddRange(dataPoints.Select(x => new DataPoint(GetXValue(x), WheelSelectionFunction(x.PlayerData.CarInfo.WheelsInfo).TyreCoreTemperature.ActualQuantity.GetValueInUnits(TemperatureUnits))));

            double newMax =Math.Max(Math.Max(Math.Max(lineSeries[0].Points.Max(x => x.Y), lineSeries[1].Points.Max(x => x.Y)), lineSeries[2].Points.Max(x => x.Y)), lineSeries[3].Points.Max(x => x.Y)) *1.1;
            if (newMax > YMaximum)
            {
                YMaximum = newMax;
            }

            return lineSeries.ToList();
        }

        protected override void ApplyNewLineColor(List<LineSeries> series, OxyColor newColor)
        {
            series.First(x => x.Title.Contains(LeftTemperatureName)).Color = newColor.ChangeSaturation(0.5);
            series.First(x => x.Title.Contains(RightTemperatureName)).Color = newColor.ChangeSaturation(1.5);
            series.First(x => x.Title.Contains(MiddleTemperatureName)).Color = newColor;
            series.First(x => x.Title.Contains(CoreTemperatureName)).Color = newColor.ChangeIntensity(0.5);
        }

        private void GraphViewSynchronization_TyreTempVisibilityChanged(object sender, Controllers.Synchronization.Graphs.TyreTempVisibilityArgs e)
        {
            if (!SyncWithOtherGraphs)
            {
                return;
            }

            _leftTemperatureVisible = e.LeftTempVisible;
            _rightTemperatureVisible = e.RightTempVisible;
            _coreTemperatureVisible = e.CoreTempVisible;
            _middleTemperatureVisible = e.MiddleTempVisible;

            NotifyPropertyChanged(nameof(LeftTemperatureVisible));
            NotifyPropertyChanged(nameof(MiddleTemperatureVisible));
            NotifyPropertyChanged(nameof(RightTemperatureVisible));
            NotifyPropertyChanged(nameof(CoreTemperatureVisible));

            UpdateSeriesVisibility();
        }

        private void UpdateSeriesVisibility()
        {
            foreach (List<LineSeries> lineSeries in LoadedSeries.Values.Select(x => x.lineSeries))
            {
                foreach (LineSeries series in lineSeries)
                {
                    if (series.Title.Contains(LeftTemperatureName))
                    {
                        series.IsVisible = LeftTemperatureVisible;
                    }

                    if (series.Title.Contains(MiddleTemperatureName))
                    {
                        series.IsVisible = MiddleTemperatureVisible;
                    }

                    if (series.Title.Contains(RightTemperatureName))
                    {
                        series.IsVisible = RightTemperatureVisible;
                    }

                    if (series.Title.Contains(CoreTemperatureName))
                    {
                        series.IsVisible = CoreTemperatureVisible;
                    }
                }
            }

            InvalidatePlot();
        }

        private void NotifyTyreTempVisibilityChanged()
        {
            if (!SyncWithOtherGraphs)
            {
                return;
            }

            GraphViewSynchronization.NotifyTyreTempVisibilityChanged(this, LeftTemperatureVisible, MiddleTemperatureVisible, RightTemperatureVisible, CoreTemperatureVisible);
        }
    }
}