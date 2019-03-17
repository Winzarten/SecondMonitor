namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Inputs
{
    using System.Collections.Generic;
    using DataExtractor;
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class ClutchGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        public ClutchGraphViewModel(IEnumerable<ISingleSeriesDataExtractor> dataExtractors) : base(dataExtractors)
        {
        }

        public override string Title => "Clutch";
        protected override string YUnits => "%";
        protected override double YTickInterval => 25;
        protected override bool CanYZoom => true;

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