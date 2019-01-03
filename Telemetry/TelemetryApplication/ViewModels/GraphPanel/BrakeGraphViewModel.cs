namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System;
    using DataModel.Telemetry;

    public class BrakeGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        public BrakeGraphViewModel()
        {
        }

        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return value.InputInfo.BrakePedalPosition * 100;
        }

        protected override void SetInitialYMaximum()
        {
            YMaximum = 101;
        }

        protected override string Title => "Brake";
        protected override string YUnits => "%";
        protected override double YTickInterval => 20;
        protected override bool CanYZooM => false;

        //protected override bool FilterFunction(TimedTelemetrySnapshot previousSnapshot, TimedTelemetrySnapshot currentSnapshot) => Math.Abs(previousSnapshot.InputInfo.BrakePedalPosition - currentSnapshot.InputInfo.BrakePedalPosition) > 0.01;

    }
}