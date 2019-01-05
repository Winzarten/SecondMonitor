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
        public override string Title => TyrePrefix + " Tyre Temperatures";
        protected abstract string TyrePrefix { get; }
        protected override string YUnits => Temperature.GetUnitSymbol(TemperatureUnits);
        protected override double YTickInterval => 20;
        protected override bool CanYZoom => true;

        private static readonly LineStyle LeftSideLineStyle = LineStyle.Solid;
        private static readonly LineStyle RightSideLineStyle = LineStyle.Solid;
        private static readonly LineStyle CenterSideLineStyle = LineStyle.Solid;
        private static readonly LineStyle CoreSideLineStyle = LineStyle.Solid;

        protected abstract Func<Wheels, WheelInfo> WheelSelectionFunction { get; }

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {

        }

        protected override List<LineSeries> GetLineSeries(LapSummaryDto lapSummary, TimedTelemetrySnapshot[] dataPoints, OxyColor color)
        {
            LineSeries[] lineSeries = new LineSeries[4];

            string baseTitle = $"Lap {lapSummary.LapNumber}";
            lineSeries[0] = CreateLineSeries(baseTitle + " Left", color, LeftSideLineStyle);
            lineSeries[1] = CreateLineSeries(baseTitle + " Right", color, RightSideLineStyle);
            lineSeries[2] = CreateLineSeries(baseTitle + " Center", color, CenterSideLineStyle);
            lineSeries[3] = CreateLineSeries(baseTitle + " Core", color, CoreSideLineStyle);

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
    }
}