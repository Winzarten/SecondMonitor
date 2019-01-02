namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System;

    public abstract class AbstractDoubleGraphViewModel : AbstractGraphViewModel<double>
    {
        protected override double GetYValue(double value, int index) => value;

        public override Func<double, string> YFormatter => (d) => d.ToString("N2");
    }
}