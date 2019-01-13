namespace SecondMonitor.Telemetry.TelemetryApplication.Settings
{
    using DTO;

    public interface ITelemetrySettingsRepository
    {
        TelemetrySettingsDto LoadOrCreateNew();
        bool TryLoadTelemetrySettings(out TelemetrySettingsDto telemetrySettings);
        void SaveTelemetrySettings(TelemetrySettingsDto telemetrySettings);
    }
}