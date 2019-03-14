namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Inputs
{
    using System.Collections.Generic;
    using System.Linq;
    using DataExtractor;
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class SteeringAngleGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        public SteeringAngleGraphViewModel(IEnumerable<ISingleSeriesDataExtractor> dataExtractors) : base(dataExtractors)
        {
        }

        public override string Title => "Steering Angle";
        protected override string YUnits => "°";
        protected override double YTickInterval => 45;
        protected override bool CanYZoom => true;
        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {
            double max = lapTelemetry.TimedTelemetrySnapshots.Max(x => x.InputInfo.WheelAngle);
            if (max > YMaximum)
            {
                YMinimum = -max;
                YMaximum = max;
            }
        }

        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return value.InputInfo.WheelAngle;
        }
    }
}