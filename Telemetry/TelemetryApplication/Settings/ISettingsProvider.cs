namespace SecondMonitor.Telemetry.TelemetryApplication.Settings
{
    using SecondMonitor.ViewModels.Settings.ViewModel;

    public interface ISettingsProvider
    {
        DisplaySettingsViewModel DisplaySettingsViewModel { get; }

        IGraphsSettingsProvider GraphsSettingsProvider { get; }

        string TelemetryRepositoryPath { get; }

        string MapRepositoryPath { get; }
    }
}