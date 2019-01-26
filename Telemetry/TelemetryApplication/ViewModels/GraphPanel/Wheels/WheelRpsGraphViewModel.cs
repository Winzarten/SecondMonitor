namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels
{
    using System;
    using DataModel.Snapshot.Systems;

    public class WheelRpsGraphViewModel : AbstractWheelsGraphViewModel
    {
        public override string Title => "Tyre Rps";
        protected override string YUnits => "Rad / s";
        protected override double YTickInterval => 100;
        protected override bool CanYZoom => true;
        protected override Func<WheelInfo, double> ExtractorFunction => x => x.Rps;
    }
}