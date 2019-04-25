namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System;
    using System.Collections.Generic;
    using DataExtractor;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class RakeGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        private double _maxRake;
        private double _minRake;
        public RakeGraphViewModel(IEnumerable<ISingleSeriesDataExtractor> dataExtractors) : base(dataExtractors)
        {
            _maxRake = 0;
            _minRake = double.MaxValue;
        }

        public override string Title => "Rake";
        protected override string YUnits => Distance.GetUnitsSymbol(SuspensionDistanceUnits);
        protected override double YTickInterval => Math.Round(Distance.FromMeters(0.01).GetByUnit(SuspensionDistanceUnits), 2);
        protected override bool CanYZoom => true;

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {
            if (_maxRake != 0)
            {
                YMaximum = _maxRake;
            }

            if (_minRake < double.MaxValue)
            {
                YMinimum = _minRake;
            }
        }

        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            double frontHeight = 0;
            double rearHeight= 0;
            if (value.PlayerData.CarInfo.FrontHeight != null && value.PlayerData.CarInfo.RearHeight != null && !value.PlayerData.CarInfo.FrontHeight.IsZero)
            {
                rearHeight = value.PlayerData.CarInfo.RearHeight.GetByUnit(SuspensionDistanceUnits);
                frontHeight = value.PlayerData.CarInfo.FrontHeight.GetByUnit(SuspensionDistanceUnits);
            }else if (value.PlayerData.CarInfo.WheelsInfo?.FrontLeft?.RideHeight != null )
            {
                DataModel.Snapshot.Systems.Wheels wheels = value.PlayerData.CarInfo.WheelsInfo;
                frontHeight = (wheels.FrontLeft.RideHeight.GetByUnit(SuspensionDistanceUnits) + wheels.FrontRight.RideHeight.GetByUnit(SuspensionDistanceUnits)) / 2;
                rearHeight = (wheels.RearLeft.RideHeight.GetByUnit(SuspensionDistanceUnits) + wheels.RearRight.RideHeight.GetByUnit(SuspensionDistanceUnits)) / 2;

            }

            double rake = rearHeight - frontHeight;
            _maxRake = Math.Max(_maxRake, rake);
            return rake;
        }
    }
}