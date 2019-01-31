using SecondMonitor.SimdataManagement.SimSettings;

namespace SecondMonitor.Timing.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using WindowsControls.Extension;
    using DataModel.Snapshot;
    using PluginManager.Core;
    using PluginManager.GameConnector;
    using LapTimings.ViewModel;
    using Presentation.View;
    using Presentation.ViewModel;
    using WindowsControls.WPF.Commands;
    using SecondMonitor.Telemetry.TelemetryApplication.Controllers;
    using SecondMonitor.Telemetry.TelemetryManagement.Repository;
    using SimdataManagement;
    using SimdataManagement.DriverPresentation;
    using Telemetry;
    using TelemetryPresentation.MainWindow;
    using ViewModels.Settings;
    using ViewModels.Settings.ViewModel;

    public class TimingApplicationController : ISecondMonitorPlugin
    {
        private const string MapFolder = "TrackMaps";
        private const string SettingsFolder = "Settings";
        private const string TelemetryFolder = "Telemetry";
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SecondMonitor\\settings.json");

        private TimingDataViewModel _timingDataViewModel;
        private SimSettingController _simSettingController;
        private DisplaySettingsWindow _settingsWindow;
        private PluginsManager _pluginsManager;
        private TimingGui _timingGui;
        private DisplaySettingsViewModel _displaySettingsViewModel;
        private MapManagementController _mapManagementController;
        private DriverPresentationsManager _driverPresentationsManager;
        private ISessionTelemetryControllerFactory _sessionTelemetryControllerFactory;
        private DisplaySettingAutoSaver _settingAutoSaver;


        public TimingApplicationController()
        {
        }

        public PluginsManager PluginManager
        {
            get => _pluginsManager;
            set
            {
                _pluginsManager = value;
                _pluginsManager.DataLoaded += OnDataLoaded;
                _pluginsManager.SessionStarted += OnSessionStarted;
                _pluginsManager.DisplayMessage += DisplayMessage;
            }
        }

        public bool IsDaemon => false;

        public void RunPlugin()
        {
            CreateDisplaySettingsViewModel();
            CreateAutoSaver();
            CreateSimSettingsController();
            CreateMapManagementController();
            CreateDriverPresentationManager();
            CreateSessionTelemetryControllerFactory();
            DriverLapsWindowManager driverLapsWindowManager = new DriverLapsWindowManager(() => _timingGui, () => _timingDataViewModel.SelectedDriverTiming);
            _timingDataViewModel = new TimingDataViewModel(driverLapsWindowManager, _displaySettingsViewModel, _driverPresentationsManager, _sessionTelemetryControllerFactory) {MapManagementController = _mapManagementController};
            BindCommands();
            CreateGui();
            _timingDataViewModel.GuiDispatcher = _timingGui.Dispatcher;
            _timingDataViewModel?.Reset();
        }

        private void CreateSessionTelemetryControllerFactory()
        {
            _sessionTelemetryControllerFactory = new SessionTelemetryControllerFactory(new TelemetryRepository(Path.Combine(_displaySettingsViewModel.ReportingSettingsView.ExportDirectoryReplacedSpecialDirs, TelemetryFolder), _displaySettingsViewModel.TelemetrySettingsViewModel.MaxSessionsKept));
        }


        private void DisplayMessage(object sender, MessageArgs e)
        {
            _timingDataViewModel?.DisplayMessage(e);
        }

        private void OnSessionStarted(object sender, DataEventArgs e)
        {
            _timingDataViewModel?.StartNewSession(e.Data);
        }

        private void OnDataLoaded(object sender, DataEventArgs e)
        {
            SimulatorDataSet dataSet = e.Data;
            try
            {
                _simSettingController?.ApplySimSettings(dataSet);

            }
            catch (SimSettingsException ex)
            {
                _timingDataViewModel.DisplayMessage(new MessageArgs(ex.Message));
                _simSettingController?.ApplySimSettings(dataSet);
            }

            _timingDataViewModel.ApplyDateSet(dataSet);
        }

        private void CreateSimSettingsController()
        {
            _simSettingController = new SimSettingController(_displaySettingsViewModel);
        }

        private void CreateGui()
        {
            _timingGui = new TimingGui(false);
            _timingGui.Show();
            _timingGui.Closed += OnGuiClosed;
            _timingGui.MouseLeave += GuiOnMouseLeave;
            _timingGui.DataContext = _timingDataViewModel;
        }

        private async void OnGuiClosed(object sender, EventArgs e)
        {
            _timingGui = null;
            List<Exception> exceptions = new List<Exception>();
            _timingDataViewModel?.TerminatePeriodicTask(exceptions);
            await _pluginsManager.DeletePlugin(this, exceptions);
        }

        private void GuiOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            if (_timingGui != null)
            {
                _timingGui.DtTimig.SelectedItem = null;
            }
        }

        private void BindCommands()
        {
            _timingDataViewModel.RightClickCommand = new RelayCommand(UnSelectItem);
            _timingDataViewModel.OpenSettingsCommand = new RelayCommand(OpenSettingsWindow);
            _timingDataViewModel.ScrollToPlayerCommand = new RelayCommand(ScrollToPlayer);
            _timingDataViewModel.OpenCarSettingsCommand = new RelayCommand(OpenCarSettingsWindow);
            _timingDataViewModel.OpenCurrentTelemetrySession = new AsyncCommand(OpenCurrentTelemetrySession);
        }

        private void UnSelectItem()
        {
            _timingDataViewModel.SelectedDriverTimingViewModel = null;
        }

        private void CreateDisplaySettingsViewModel()
        {
           _displaySettingsViewModel = new DisplaySettingsViewModel();
           _displaySettingsViewModel.FromModel(
                new DisplaySettingsLoader().LoadDisplaySettingsFromFileSafe(SettingsPath));
        }

        private void CreateMapManagementController()
        {
            _mapManagementController = new MapManagementController(new TrackMapFromTelemetryFactory(TimeSpan.FromMilliseconds(_displaySettingsViewModel.MapDisplaySettingsViewModel.MapPointsInterval),100),
                new MapsLoader(Path.Combine(_displaySettingsViewModel.ReportingSettingsView.ExportDirectoryReplacedSpecialDirs, MapFolder)),
                new TrackDtoManipulator());
        }

        private void CreateDriverPresentationManager()
        {
            _driverPresentationsManager = new DriverPresentationsManager(new DriverPresentationsLoader(Path.Combine(_displaySettingsViewModel.ReportingSettingsView.ExportDirectoryReplacedSpecialDirs, SettingsFolder)));
        }

        private void OpenSettingsWindow()
        {
            if (_settingsWindow != null && _settingsWindow.IsVisible)
            {
                _settingsWindow.Focus();
                return;
            }

            _settingsWindow = new DisplaySettingsWindow
                                  {
                                      DataContext = _displaySettingsViewModel,
                                      Owner = _timingGui
                                  };
            _settingsWindow.Show();
        }

        private async Task OpenCurrentTelemetrySession()
        {
            MainWindow mainWindow = new MainWindow();
            TelemetryApplicationController controller = new TelemetryApplicationController(mainWindow);
            await controller.StartControllerAsync();
            mainWindow.Closed += async (sender, args) =>
            {
                await controller.StopControllerAsync();
            };
            await controller.OpenLastSessionFromRepository();
        }

        private void OpenCarSettingsWindow()
        {
            _simSettingController.OpenCarSettingsControl(_timingGui);
        }

        private void CreateAutoSaver()
        {
            _settingAutoSaver = new DisplaySettingAutoSaver(SettingsPath) { DisplaySettingsViewModel = _displaySettingsViewModel };
        }

        private void ScrollToPlayer()
        {
            if (_displaySettingsViewModel.ScrollToPlayer && _timingGui != null && _timingDataViewModel?.SessionTiming?.Player != null && _timingGui.DtTimig.Items.Count > 0)
            {
                _timingGui.DtTimig.ScrollToCenterOfView(_timingDataViewModel.SessionTiming.Player);
            }
        }
    }
}