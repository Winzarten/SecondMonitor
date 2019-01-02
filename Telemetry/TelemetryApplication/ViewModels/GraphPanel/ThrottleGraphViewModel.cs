namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System.Linq;

    public class ThrottleGraphViewModel : AbstractDoubleGraphViewModel
    {

        protected override double[] GetDataPoints() => OriginalModel.TimedTelemetrySnapshots.Select(x => x.InputInfo.ThrottlePedalPosition * 100).ToArray();
    }
}