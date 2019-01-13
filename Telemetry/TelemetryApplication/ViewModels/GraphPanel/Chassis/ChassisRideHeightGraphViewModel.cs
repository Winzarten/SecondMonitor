namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Chassis
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;

    public class ChassisRideHeightGraphViewModel : AbstractChassisGraphViewModel
    {
        public override string Title => "Ride Height - Chassis";
        protected override string YUnits => Distance.GetUnitsSymbol(SuspensionDistanceUnits);
        protected override double YTickInterval => Distance.FromMeters(0.02).GetByUnit(SuspensionDistanceUnits);
        protected override Func<CarInfo, double> FrontFunc => (x) => x.FrontHeight?.GetByUnit(SuspensionDistanceUnits) ?? 0;
        protected override Func<CarInfo, double> RearFunc => (x) => x.RearHeight?.GetByUnit(SuspensionDistanceUnits) ?? 0;
    }
}