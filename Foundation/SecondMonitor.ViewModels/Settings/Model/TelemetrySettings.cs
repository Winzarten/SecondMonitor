namespace SecondMonitor.ViewModels.Settings.Model
{
    using System;

    [Serializable]
    public class TelemetrySettings
    {
        public bool IsFeatureEnabled { get; set; } = false;
        public bool IsTelemetryLoggingEnabled { get; set; } = false;
        public int LoggingInterval { get; set; } = 50;
        public int MaxSessionsKept { get; set; } = 10;
    }
}