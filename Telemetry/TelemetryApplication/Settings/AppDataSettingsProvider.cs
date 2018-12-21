namespace SecondMonitor.Telemetry.TelemetryApplication.Settings
{
    using System;
    using System.IO;
    using System.Threading;
    using SecondMonitor.ViewModels.Settings.ViewModel;

    public class AppDataSettingsProvider : ISettingsProvider
    {
        private const string MapFolder = "TrackMaps";
        private const string SettingsFolder = "Settings";
        private const string TelemetryFolder = "Telemetry_Samples";
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SecondMonitor\\settings.json");

        private readonly Lazy<DisplaySettingsViewModel> _displaySettingsLazy;

        public AppDataSettingsProvider()
        {
            _displaySettingsLazy = new Lazy<DisplaySettingsViewModel>(LoadSettings, LazyThreadSafetyMode.PublicationOnly);
        }

        public DisplaySettingsViewModel DisplaySettingsViewModel => _displaySettingsLazy.Value;

        public string TelemetryRepositoryPath => Path.Combine(DisplaySettingsViewModel.ReportingSettingsView.ExportDirectoryReplacedSpecialDirs, TelemetryFolder);

        private DisplaySettingsViewModel LoadSettings()
        {
            DisplaySettingsViewModel displaySettingsViewModel = new DisplaySettingsViewModel();
            displaySettingsViewModel.FromModel(
                new DisplaySettingsLoader().LoadDisplaySettingsFromFileSafe(SettingsPath));
            return displaySettingsViewModel;
        }
    }
}