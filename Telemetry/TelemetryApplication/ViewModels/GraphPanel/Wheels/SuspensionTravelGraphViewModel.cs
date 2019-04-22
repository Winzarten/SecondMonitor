namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;

    public class SuspensionTravelGraphViewModel : AbstractWheelsGraphViewModel
    {
        public override string Title => "Suspension Travel";
        protected override string YUnits => Distance.GetUnitsSymbol(SuspensionDistanceUnits);
        protected override double YTickInterval => Math.Round(Distance.FromMeters(0.01).GetByUnit(SuspensionDistanceUnits), 2);
        protected override bool CanYZoom => true;
        protected override Func<WheelInfo, double> ExtractorFunction => x => x.SuspensionTravel?.GetByUnit(SuspensionDistanceUnits) ?? 0;
    }
}