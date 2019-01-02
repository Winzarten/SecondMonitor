namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels
{
    using DataModel.Telemetry;
    using SecondMonitor.ViewModels;

    public abstract class SnapshotViewModel : AbstractViewModel<TimedTelemetrySnapshot>, ISnapshotViewModel
    {
        public override TimedTelemetrySnapshot SaveToNewModel()
        {
            return OriginalModel;
        }
    }
}