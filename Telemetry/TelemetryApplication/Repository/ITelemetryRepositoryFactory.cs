namespace SecondMonitor.Telemetry.TelemetryApplication.Repository
{
    using SecondMonitor.ViewModels.Settings;
    using Settings;
    using TelemetryManagement.Repository;

    public interface ITelemetryRepositoryFactory
    {
        ITelemetryRepository Create(ISettingsProvider settingsProvider);
    }
}