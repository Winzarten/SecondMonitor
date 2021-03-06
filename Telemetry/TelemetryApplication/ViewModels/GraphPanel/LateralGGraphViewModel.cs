﻿namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System.Collections.Generic;
    using DataExtractor;
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class LateralGGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        public LateralGGraphViewModel(IEnumerable<ISingleSeriesDataExtractor> dataExtractors) : base(dataExtractors)
        {
        }

        public override string Title => "Lateral Acceleration";
        protected override string YUnits => "Gs";
        protected override double YTickInterval => 1;
        protected override bool CanYZoom => true;
        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {
            YMinimum = -3;
            YMaximum = 3;
        }

        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return value.PlayerData.CarInfo.Acceleration.XinG;
        }
    }
}