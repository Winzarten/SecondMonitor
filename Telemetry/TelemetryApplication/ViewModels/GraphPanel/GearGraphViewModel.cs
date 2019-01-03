namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using DataModel.Telemetry;

    public class GearGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        protected override string Title => "Gear";
        protected override string YUnits => "Gear";
        protected override double YTickInterval => 1;
        protected override bool CanYZooM => true;

        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            switch (value.PlayerData.CarInfo.CurrentGear)
            {
                case "R":
                    return -1;
                case "N":
                    return 0;
                default:
                    return double.Parse(value.PlayerData.CarInfo.CurrentGear);
            }
        }

        protected override void SetInitialYMaximum()
        {
            YMaximum = 7;
        }
    }
}