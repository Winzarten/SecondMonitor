namespace SecondMonitor.Telemetry.TelemetryApplication.Settings
{
    using System;
    using DTO;

    public interface ITelemetrySettingsRepository
    {
        event EventHandler<EventArgs> SettingsChanged;

        TelemetrySettingsDto LoadOrCreateNew();
        bool TryLoadTelemetrySettings(out TelemetrySettingsDto telemetrySettings);
        void SaveTelemetrySettings(TelemetrySettingsDto telemetrySettings);
    }
}