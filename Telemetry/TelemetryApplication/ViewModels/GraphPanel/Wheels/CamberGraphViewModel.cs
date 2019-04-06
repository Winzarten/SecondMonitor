namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;

    public class CamberGraphViewModel : AbstractWheelsGraphViewModel
    {
        public override string Title => "Camber";
        protected override string YUnits => Angle.GetUnitsSymbol(AngleUnits);
        protected override double YTickInterval => Angle.GetFromDegrees(1).GetValueInUnits(AngleUnits);
        protected override bool CanYZoom => true;
        protected override Func<WheelInfo, double> ExtractorFunction => x => x?.Camber?.GetValueInUnits(AngleUnits) ?? 0.0;
    }
}