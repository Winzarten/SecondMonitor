namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels
{
    using System;
    using DataModel.Snapshot.Systems;

    public class RightFrontTyreTempsGraphViewModel : AbstractTyreTemperaturesViewModel
    {
        protected override string TyrePrefix => "Right Front";
        protected override Func<Wheels, WheelInfo> WheelSelectionFunction => x => x.FrontRight;
    }
}