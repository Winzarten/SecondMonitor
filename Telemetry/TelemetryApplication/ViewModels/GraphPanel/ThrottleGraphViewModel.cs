﻿namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System;
    using DataModel.Telemetry;

    public class ThrottleGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return value.InputInfo.ThrottlePedalPosition * 100;
        }

        protected override void SetInitialYMaximum()
        {
            YMaximum = 101;
        }

        protected override string Title => "Throttle";
        protected override string YUnits => "%";
        protected override double YTickInterval => 20;
        protected override bool CanYZooM => false;

        //protected override bool FilterFunction(TimedTelemetrySnapshot previousSnapshot, TimedTelemetrySnapshot currentSnapshot) => Math.Abs(previousSnapshot.InputInfo.ThrottlePedalPosition - currentSnapshot.InputInfo.ThrottlePedalPosition) > 0.01;
    }
}