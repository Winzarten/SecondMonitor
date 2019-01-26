namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;

    public class TyrePressuresGraphViewModel : AbstractWheelsGraphViewModel
    {
        public override string Title => "Tyre Pressures";
        protected override string YUnits => Pressure.GetUnitSymbol(PressureUnits);
        protected override double YTickInterval => Pressure.FromKiloPascals(20).GetValueInUnits(PressureUnits);
        protected override bool CanYZoom => true;
        protected override Func<WheelInfo, double> ExtractorFunction => (x) => x.TyrePressure.ActualQuantity.GetValueInUnits(PressureUnits);
    }
}