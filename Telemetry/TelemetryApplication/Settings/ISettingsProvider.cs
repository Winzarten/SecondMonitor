namespace SecondMonitor.Telemetry.TelemetryApplication.Settings
{
    using SecondMonitor.ViewModels.Settings.ViewModel;

    public interface ISettingsProvider
    {
        DisplaySettingsViewModel DisplaySettingsViewModel { get; }

        string TelemetryRepositoryPath { get; }
    }
}