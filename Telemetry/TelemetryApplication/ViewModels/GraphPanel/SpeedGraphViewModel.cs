namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System.Linq;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class SpeedGraphViewModel : AbstractSingleSeriesGraphViewModel

    {
        public override string Title => "Speed";
        protected override string YUnits  => Velocity.GetUnitSymbol(VelocityUnits);
        protected override double YTickInterval => 50;
        protected override bool CanYZoom => true;

        protected override double GetYValue(TimedTelemetrySnapshot value)
        {
            return value.PlayerData.Speed.GetValueInUnits(VelocityUnits);
        }

        protected override void UpdateYMaximum(LapTelemetryDto lapTelemetry)
        {
            double newMax = lapTelemetry.TimedTelemetrySnapshots.Max(x => x.PlayerData.Speed.GetValueInUnits(VelocityUnits)) * 1.1;
            if (newMax > YMaximum)
            {
                YMaximum = newMax;
            }
        }
    }
}