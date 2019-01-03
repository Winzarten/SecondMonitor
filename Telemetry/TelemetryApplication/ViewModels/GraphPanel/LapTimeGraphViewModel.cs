﻿namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class LapTimeGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        protected override string Title => "Lap Time";
        protected override string YUnits => "Seconds";
        protected override double YTickInterval => 20;
        protected override bool CanYZooM => true;

        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return value.LapTimeSeconds;
        }

        protected override void SetInitialYMaximum()
        {

        }

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {
            double newMax = lapTelemetry.LapSummary.LapTimeSeconds * 1.1;
            if (newMax > YMaximum)
            {
                YMaximum = newMax;
            }
        }
    }
}