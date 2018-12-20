namespace SecondMonitor.Telemetry.TelemetryApplication.Repository
{
    using Settings;
    using TelemetryManagement.Repository;

    public class TelemetryRepositoryFactory : ITelemetryRepositoryFactory
    {
        public ITelemetryRepository Create(ISettingsProvider settingsProvider)
        {
            return new TelemetryRepository(settingsProvider.TelemetryRepositoryPath, settingsProvider.DisplaySettingsViewModel.TelemetrySettingsViewModel.MaxSessionsKept);
        }
    }
}