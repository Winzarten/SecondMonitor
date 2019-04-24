namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;

    public class SuspensionVelocityGraphViewModel : AbstractWheelsGraphViewModel
    {
        public override string Title => "Suspension Velocity";

        protected override string YUnits => Velocity.GetUnitSymbol(VelocityUnitsSmall);

        protected override double YTickInterval => Math.Round(Velocity.FromMs(0.05).GetValueInUnits(VelocityUnitsSmall), 2);

        protected override bool CanYZoom => true;

        protected override Func<WheelInfo, double> ExtractorFunction => (x) => x.SuspensionVelocity?.GetValueInUnits(VelocityUnitsSmall) ?? 0;
    }
}