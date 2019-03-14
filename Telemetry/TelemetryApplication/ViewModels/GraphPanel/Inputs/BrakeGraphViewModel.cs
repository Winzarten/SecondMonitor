namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Inputs
{
    using System.Collections.Generic;
    using DataExtractor;
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class BrakeGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        public BrakeGraphViewModel(IEnumerable<ISingleSeriesDataExtractor> dataExtractors) : base(dataExtractors)
        {
        }

        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return value.InputInfo.BrakePedalPosition * 100;
        }

        public override string Title => "Brake";
        protected override string YUnits => "%";
        protected override double YTickInterval => 20;
        protected override bool CanYZoom => true;

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {
            YMaximum = 101;
        }

        //protected override bool FilterFunction(TimedTelemetrySnapshot previousSnapshot, TimedTelemetrySnapshot currentSnapshot) => Math.Abs(previousSnapshot.InputInfo.BrakePedalPosition - currentSnapshot.InputInfo.BrakePedalPosition) > 0.01;

    }
}