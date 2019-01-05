namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Inputs
{
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class ThrottleGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        public override string Title => "Throttle";
        protected override string YUnits => "%";
        protected override double YTickInterval => 20;
        protected override bool CanYZoom => false;

        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return value.InputInfo.ThrottlePedalPosition * 100;
        }

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {
            YMaximum = 101;
        }

        //protected override bool FilterFunction(TimedTelemetrySnapshot previousSnapshot, TimedTelemetrySnapshot currentSnapshot) => Math.Abs(previousSnapshot.InputInfo.ThrottlePedalPosition - currentSnapshot.InputInfo.ThrottlePedalPosition) > 0.01;
    }
}