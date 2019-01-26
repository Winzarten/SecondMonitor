namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Systems;

    public class BrakeTemperaturesGraphViewModel : AbstractWheelsGraphViewModel
    {
        public override string Title => "Brake Temperature";
        protected override string YUnits => Temperature.GetUnitSymbol(TemperatureUnits);
        protected override double YTickInterval => 100;
        protected override bool CanYZoom => true;
        protected override Func<WheelInfo, double> ExtractorFunction => (x) => x.BrakeTemperature.ActualQuantity.GetValueInUnits(TemperatureUnits);
    }
}