namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using DataModel.Telemetry;

    public class LateralGGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        protected override string Title => "Lateral Acceleration";
        protected override string YUnits => "Gs";
        protected override double YTickInterval => 1;
        protected override bool CanYZooM => true;
        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return value.PlayerData.CarInfo.Acceleration.XinG;
        }

        protected override void SetInitialYMaximum()
        {
            YMinimum = -3;
            YMaximum = 3;
        }
    }
}