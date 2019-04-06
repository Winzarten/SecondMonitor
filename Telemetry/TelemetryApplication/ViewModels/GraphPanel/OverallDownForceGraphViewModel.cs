namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System.Collections.Generic;
    using System.Linq;
    using DataExtractor;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class OverallDownForceGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        public OverallDownForceGraphViewModel(IEnumerable<ISingleSeriesDataExtractor> dataExtractors) : base(dataExtractors)
        {
        }

        public override string Title => "Overall DownForce";
        protected override string YUnits => Force.GetUnitSymbol(ForceUnits);
        protected override double YTickInterval => Force.GetFromNewtons(1000).GetValueInUnits(ForceUnits);
        protected override bool CanYZoom => true;
        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {
            double newYMax = lapTelemetry.TimedTelemetrySnapshots.Max(x => x.PlayerData?.CarInfo?.OverallDownForce?.GetValueInUnits(ForceUnits) ?? 0.0);
            if (newYMax > YMaximum)
            {
                YMaximum = newYMax;
            }
        }

        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return value.PlayerData?.CarInfo?.OverallDownForce?.GetValueInUnits(ForceUnits) ?? 0.0;
        }
    }
}