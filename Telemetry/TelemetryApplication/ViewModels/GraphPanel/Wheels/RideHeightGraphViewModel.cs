namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;

    public class RideHeightGraphViewModel : AbstractWheelsGraphViewModel
    {
        public override string Title => "Ride Height - Wheels";
        protected override string YUnits => Distance.GetUnitsSymbol(SuspensionDistanceUnits);
        protected override double YTickInterval => Distance.FromMeters(0.02).GetByUnit(SuspensionDistanceUnits);
        protected override bool CanYZoom => true;
        protected override Func<WheelInfo, double> ExtractorFunction => x => x.RideHeight?.GetByUnit(SuspensionDistanceUnits) ?? 0;
    }
}