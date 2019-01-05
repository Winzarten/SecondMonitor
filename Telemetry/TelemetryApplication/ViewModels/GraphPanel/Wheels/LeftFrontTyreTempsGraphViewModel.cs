namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels
{
    using System;
    using DataModel.Snapshot.Systems;

    public class LeftFrontTyreTempsGraphViewModel : AbstractTyreTemperaturesViewModel
    {
        protected override string TyrePrefix => "Left Front";
        protected override Func<Wheels, WheelInfo> WheelSelectionFunction => (x) => x.FrontLeft;
    }
}