namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels
{
    using DataModel.Telemetry;
    using SecondMonitor.ViewModels;

    public abstract class AbstractSnapshotViewModel : AbstractViewModel<TimedTelemetrySnapshot>, IAbstractSnapshotViewModel
    {
        public override TimedTelemetrySnapshot SaveToNewModel()
        {
            return OriginalModel;
        }
    }
}