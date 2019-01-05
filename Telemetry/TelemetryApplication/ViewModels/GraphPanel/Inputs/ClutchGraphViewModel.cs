namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Inputs
{
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class ClutchGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        public override string Title => "Clutch";
        protected override string YUnits => "%";
        protected override double YTickInterval => 25;
        protected override bool CanYZoom => false;

       protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return value.InputInfo.ClutchPedalPosition * 100;
        }

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {
            YMaximum = 101;
        }
    }
}