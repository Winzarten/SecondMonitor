namespace SecondMonitor.Telemetry.TelemetryApplication.Repository
{
    using Settings;
    using TelemetryManagement.Repository;

    public interface ITelemetryRepositoryFactory
    {
        ITelemetryRepository Create(ISettingsProvider settingsProvider);
    }
}