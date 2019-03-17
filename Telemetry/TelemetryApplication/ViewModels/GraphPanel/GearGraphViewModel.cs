namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System.Collections.Generic;
    using DataExtractor;
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class GearGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        public GearGraphViewModel(IEnumerable<ISingleSeriesDataExtractor> dataExtractors) : base(dataExtractors)
        {
        }

        public override string Title => "Gear";
        protected override string YUnits => "Gear";
        protected override double YTickInterval => 1;
        protected override bool CanYZoom => true;


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

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {
            YMaximum = 7;
        }
    }
}