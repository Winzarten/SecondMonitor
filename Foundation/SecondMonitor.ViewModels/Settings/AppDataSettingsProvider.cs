namespace SecondMonitor.ViewModels.Settings
{
    using System;
    using System.IO;
    using System.Threading;
    using ViewModel;

    public class AppDataSettingsProvider : ISettingsProvider
    {
        private const string MapFolder = "TrackMaps";
        private const string RatingsFolder = "Ratings";
        private const string SettingsFolder = "Settings";
        //private const string TelemetryFolder = "Telemetry_Samples";
        private const string TelemetryFolder = "Telemetry";
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

        public string MapRepositoryPath => Path.Combine(DisplaySettingsViewModel.ReportingSettingsView.ExportDirectoryReplacedSpecialDirs, MapFolder);
        public string RatingsRepositoryPath => Path.Combine(DisplaySettingsViewModel.ReportingSettingsView.ExportDirectoryReplacedSpecialDirs, RatingsFolder);

        private DisplaySettingsViewModel LoadSettings()
        {
            DisplaySettingsViewModel displaySettingsViewModel = new DisplaySettingsViewModel();
            displaySettingsViewModel.FromModel(
                new DisplaySettingsLoader().LoadDisplaySettingsFromFileSafe(SettingsPath));
            return displaySettingsViewModel;
        }
    }
}