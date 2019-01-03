namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using DataModel.Telemetry;

    public class ClutchGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        protected override string Title => "Clutch";
        protected override string YUnits => "%";
        protected override double YTickInterval => 25;
        protected override bool CanYZooM => false;

        protected override void SetInitialYMaximum()
        {
            YMaximum = 101;
        }

        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return value.InputInfo.ClutchPedalPosition * 100;
        }
    }
}