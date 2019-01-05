﻿namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System.Linq;
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class EngineRpmGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        public override string Title => "Engine RPM";
        protected override string YUnits => "RPM";
        protected override double YTickInterval => 1000;
        protected override bool CanYZoom => true;

        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return value.PlayerData.CarInfo.EngineRpm;
        }

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {
            double newMax = lapTelemetry.TimedTelemetrySnapshots.Max(x => x.PlayerData.CarInfo.EngineRpm) * 1.1;
            double newMin = lapTelemetry.TimedTelemetrySnapshots.Min(x => x.PlayerData.CarInfo.EngineRpm) * 0.9;
            if (newMax > YMaximum)
            {
                YMaximum = newMax;
            }

            if (newMin < YMinimum || YMinimum == 0)
            {
                YMinimum = newMin;
            }
        }
    }
}