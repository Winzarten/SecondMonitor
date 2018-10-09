namespace SecondMonitor.Timing.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Input;

    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.PluginManager.Core;
    using SecondMonitor.PluginManager.GameConnector;
    using SecondMonitor.SimdataManagement.SimSettings;
    using SecondMonitor.Timing.LapTimings.ViewModel;
    using SecondMonitor.Timing.Presentation.View;
    using SecondMonitor.Timing.Presentation.ViewModel;
    using SecondMonitor.Timing.Settings;
    using SecondMonitor.Timing.Settings.ViewModel;
    using SecondMonitor.WindowsControls.WPF.Commands;

    public class TimingApplicationController : ISecondMonitorPlugin
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SecondMonitor\\settings.json");
        private readonly TimingDataViewModel _timingDataViewModel;

        private DisplaySettingsWindow _settingsWindow;
        private PluginsManager _pluginsManager;
        private TimingGui _timingGui;
        private DisplaySettingsViewModel _displaySettingsViewModel;
        private DisplaySettingAutoSaver _settingAutoSaver;
        private SimSettingAdapter _simSettingAdapter;

        public TimingApplicationController()
        {
            DriverLapsWindowManager driverLapsWindowManager = new DriverLapsWindowManager(() => _timingGui, () => _timingDataViewModel.SelectedDriverTiming);
            _timingDataViewModel = new TimingDataViewModel(driverLapsWindowManager);
            BindCommands();
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
            CreateSimSettingsAdapter();
            CreateGui();
            _timingDataViewModel.GuiDispatcher = _timingGui.Dispatcher;
            _timingDataViewModel.DisplaySettingsViewModel = _displaySettingsViewModel;
            _timingDataViewModel?.Reset();
        }

        private void CreateSimSettingsAdapter()
        {
            _simSettingAdapter = new SimSettingAdapter(Path.Combine(_displaySettingsViewModel.ReportingSettingsView.ExportDirectoryReplacedSpecialDirs, "Settings"));
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
            _simSettingAdapter?.Visit(dataSet);
            _timingDataViewModel.ApplyDateSet(dataSet);
        }

        private void CreateGui()
        {
            _timingGui = new TimingGui();
            _timingGui.Show();
            _timingGui.Closed += Gui_Closed;
            _timingGui.MouseLeave += GuiOnMouseLeave;
            _timingGui.DataContext = _timingDataViewModel;
        }

        private void Gui_Closed(object sender, EventArgs e)
        {
            _timingGui = null;
            List<Exception> exceptions = new List<Exception>();
            _timingDataViewModel?.TerminatePeriodicTask(exceptions);
            _pluginsManager.DeletePlugin(this, exceptions);
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

        private void CreateAutoSaver()
        {
            _settingAutoSaver = new DisplaySettingAutoSaver(SettingsPath) { DisplaySettingsViewModel = _displaySettingsViewModel };
        }

        private void ScrollToPlayer()
        {
            if (_displaySettingsViewModel.ScrollToPlayer && _timingGui != null && _timingDataViewModel?.SessionTiming?.Player != null && _timingGui.DtTimig.Items.Count > 0)
            {
                _timingGui.DtTimig.ScrollIntoView(_timingGui.DtTimig.Items[0]);
                _timingGui.DtTimig.ScrollIntoView(_timingDataViewModel.SessionTiming.Player);
            }
        }
    }
}