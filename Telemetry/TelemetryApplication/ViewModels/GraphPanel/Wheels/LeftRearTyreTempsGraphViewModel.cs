namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels
{
    using System;
    using DataModel.Snapshot.Systems;

    public class LeftRearTyreTempsGraphViewModel : AbstractTyreTemperaturesViewModel
    {
        protected override string TyrePrefix => "Left Rear";
        protected override Func<Wheels, WheelInfo> WheelSelectionFunction => x => x.RearLeft;
    }
}