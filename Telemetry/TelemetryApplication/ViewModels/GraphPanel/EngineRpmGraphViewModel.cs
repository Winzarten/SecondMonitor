namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System.Linq;
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class EngineRpmGraphViewModel : AbstractSingleSeriesGraphViewModel
    {
        protected override string Title => "Engine RPM";
        protected override string YUnits => "RPM";
        protected override double YTickInterval => 1000;
        protected override bool CanYZooM => true;

        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return value.PlayerData.CarInfo.EngineRpm;
        }

        protected override void SetInitialYMaximum()
        {
            YMaximum = 2000;
        }

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {
            double newMax = lapTelemetry.TimedTelemetrySnapshots.Max(x => x.PlayerData.CarInfo.EngineRpm) * 1.1;
            if (newMax > YMaximum)
            {
                YMaximum = newMax;
            }
        }
    }
}