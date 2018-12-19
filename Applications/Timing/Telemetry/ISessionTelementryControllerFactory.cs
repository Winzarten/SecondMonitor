namespace SecondMonitor.Timing.Telemetry
{
    using DataModel.Snapshot;
    using SecondMonitor.Telemetry.TelemetryManagement.Repository;

    public class SessionTelemetryControllerFactory : ISessionTelemetryControllerFactory
    {
        private readonly ITelemetryRepository _telemetryRepository;

        public SessionTelemetryControllerFactory(ITelemetryRepository telemetryRepository)
        {
            _telemetryRepository = telemetryRepository;
        }

        public ISessionTelemetryController Create(SimulatorDataSet simulatorDataSet)
        {
            return new SessionTelemetryController(simulatorDataSet.SessionInfo.TrackInfo.TrackName, simulatorDataSet.SessionInfo.SessionType, _telemetryRepository);
        }
    }
}