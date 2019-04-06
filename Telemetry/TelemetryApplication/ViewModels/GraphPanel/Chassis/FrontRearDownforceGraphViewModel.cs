namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Chassis
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;

    public class FrontRearDownForceGraphViewModel : AbstractChassisGraphViewModel
    {
        public override string Title => "Front - Rear DownForce";
        protected override string YUnits => Force.GetUnitSymbol(ForceUnits);
        protected override double YTickInterval => Force.GetFromNewtons(1000).GetValueInUnits(ForceUnits);
        protected override bool CanYZoom => true;
        protected override Func<CarInfo, double> FrontFunc => (x) => x.FrontDownForce?.GetValueInUnits(ForceUnits) ?? 0.0;
        protected override Func<CarInfo, double> RearFunc => (x) => x.FrontDownForce?.GetValueInUnits(ForceUnits) ?? 0.0;
    }
}