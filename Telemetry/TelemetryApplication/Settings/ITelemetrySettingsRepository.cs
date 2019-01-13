namespace SecondMonitor.Telemetry.TelemetryApplication.Settings
{
    using DTO;

    public interface ITelemetrySettingsRepository
    {
        bool TryLoadTelemetrySettings(out TelemetrySettingsDto telemetrySettings);
        void SaveTelemetrySettings(TelemetrySettingsDto telemetrySettings);
    }
}