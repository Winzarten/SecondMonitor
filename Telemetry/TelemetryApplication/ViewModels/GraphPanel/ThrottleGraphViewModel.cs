namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System;
    using System.Linq;
    using DataModel.Telemetry;

    public class ThrottleGraphViewModel : AbstractGraphViewModel
    {

        protected override double GetYValue(TimedTelemetrySnapshot value, int index)
        {
            return value.InputInfo.ThrottlePedalPosition * 100;
        }

        protected override double GetXValue(TimedTelemetrySnapshot value, int index)
        {
            return value.PlayerData.LapDistance;
        }

        protected override bool FilterFunction(TimedTelemetrySnapshot previousSnapshot, TimedTelemetrySnapshot currentSnapshot) => base.FilterFunction(previousSnapshot, currentSnapshot) || Math.Abs(previousSnapshot.InputInfo.ThrottlePedalPosition  - currentSnapshot.InputInfo.ThrottlePedalPosition) > 0.01;

    }
}