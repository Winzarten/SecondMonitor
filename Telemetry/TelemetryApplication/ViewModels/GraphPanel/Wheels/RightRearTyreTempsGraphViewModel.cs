namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels
{
    using System;
    using DataModel.Snapshot.Systems;

    public class RightRearTyreTempsGraphViewModel : AbstractTyreTemperaturesViewModel
    {
        protected override string TyrePrefix => "Right Rear";
        protected override Func<Wheels, WheelInfo> WheelSelectionFunction => x => x.RearRight;
    }
}