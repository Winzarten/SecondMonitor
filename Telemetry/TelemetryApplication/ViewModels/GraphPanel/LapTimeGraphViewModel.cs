namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System.Collections.Generic;
    using System.Linq;
    using DataExtractor;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;
    using Settings.DTO;
    using TelemetryManagement.DTO;

    public class LapTimeGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        public LapTimeGraphViewModel(IEnumerable<ISingleSeriesDataExtractor> dataExtractors) : base(dataExtractors)
        {
        }

        public override string Title => "Lap Time";
        protected override string YUnits => XAxisKind == XAxisKind.LapTime ? Distance.GetUnitsSymbol(DistanceUnits) : "Seconds";
        protected override double YTickInterval => XAxisKind == XAxisKind.LapTime ? 200 : 20;
        protected override bool CanYZoom => true;

        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return XAxisKind == XAxisKind.LapTime ? Distance.FromMeters(value.PlayerData.LapDistance).GetByUnit(DistanceUnits) : value.LapTimeSeconds;
        }

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {
            double newMax = XAxisKind == XAxisKind.LapTime ? Distance.FromMeters(lapTelemetry.TimedTelemetrySnapshots.Max(x => x.PlayerData.LapDistance)).GetByUnit(DistanceUnits) : lapTelemetry.LapSummary.LapTimeSeconds * 1.1;
            if (newMax > YMaximum)
            {
                YMaximum = newMax;
            }
        }
    }
}