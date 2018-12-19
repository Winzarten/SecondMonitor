namespace SecondMonitor.Timing.Telemetry
{
    using DataModel.Snapshot;

    public interface ISessionTelemetryControllerFactory
    {
        ISessionTelemetryController Create(SimulatorDataSet simulatorDataSet);
    }
}