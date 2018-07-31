namespace SecondMonitor.Timing.Presentation.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.PluginManager.Core;
    using SecondMonitor.PluginManager.GameConnector;
    using SecondMonitor.Timing.LapTimings.ViewModel;
    using SecondMonitor.Timing.Presentation.View;
    using SecondMonitor.Timing.Presentation.ViewModel.Commands;
    using SecondMonitor.Timing.ReportCreation.ViewModel;
    using SecondMonitor.Timing.SessionTiming.Drivers;
    using SecondMonitor.Timing.SessionTiming.Drivers.ModelView;
    using SecondMonitor.Timing.SessionTiming.Drivers.Presentation.ViewModel;
    using SecondMonitor.Timing.SessionTiming.ViewModel;
    using SecondMonitor.Timing.Settings;
    using SecondMonitor.Timing.Settings.Model;
    using SecondMonitor.Timing.Settings.ModelView;

    public class TimingDataViewModel : DependencyObject, ISecondMonitorPlugin, INotifyPropertyChanged
    {

        public static readonly DependencyProperty DisplaySettingsProperty = DependencyProperty.Register("DisplaySettings", typeof(DisplaySettingsModelView), typeof(TimingDataViewModel), new PropertyMetadata(null, PropertyChangedCallback));
        public static readonly DependencyProperty CurrentSessionOptionsProperty = DependencyProperty.Register("CurrentSessionOptions", typeof(SessionOptionsModelView), typeof(TimingDataViewModel), new PropertyMetadata(null, CurrentSessionOptionsPropertyChanged));
        public static readonly DependencyProperty CurrentGearProperty = DependencyProperty.Register("CurrentGear",typeof(string), typeof(TimingDataViewModel));

        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SecondMonitor\\settings.json");

        private enum ResetModeEnum
        {
            NoReset,

            Manual,

            Automatic
        }

        private ResetModeEnum _shouldReset = ResetModeEnum.NoReset;

        private DisplaySettingAutoSaver _settingAutoSaver;

        private ReportCreationViewModel _reportCreation;

        private PluginsManager _pluginsManager;
        private SessionTiming _timing;
        private DisplaySettingsWindow _settingsWindow;
        private SessionType _sessionType = SessionType.Na;
        private SimulatorDataSet _lastDataSet;

        private string _connectedSource;
        private readonly DriverLapsWindowManager _driverLapsWindowManager;

        public TimingDataViewModel()
        {
            SessionInfoViewModel = new SessionInfoViewModel();
            _driverLapsWindowManager = new DriverLapsWindowManager(() => Gui, () => SelectedDriverTiming);
            DoubleLeftClickCommand = _driverLapsWindowManager.OpenWindowCommand;
            _reportCreation = new ReportCreationViewModel(DisplaySettings);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Gets or sets the CollectionViewSource
        public CollectionViewSource ViewSource { get; set; }

        // Gets or sets the ObservableCollection
        public ObservableCollection<DriverTimingModelView> Collection { get; set; } = new ObservableCollection<DriverTimingModelView>();

        public string SessionTime => _timing?.SessionTime.ToString("mm\\:ss\\.fff") ?? string.Empty;

        public string ConnectedSource
        {
            get => _connectedSource;
            private set
            {
                bool wasChanged = _connectedSource != value;
                _connectedSource = value;
                if (wasChanged)
                {
                    NotifyPropertyChanged();
                }
            }
        }

        public string SystemTime => DateTime.Now.ToString("HH:mm");

        public string Title
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string version = AssemblyName.GetAssemblyName(assembly.Location).Version.ToString();
                return "Second Monitor (Timing)(v" + version + " )";
            }
        }

        public SessionInfoViewModel SessionInfoViewModel { get; }

        public TimingGui Gui { get; private set; }

        public DriverTiming SelectedDriverTiming => ((DriverTimingModelView)Gui?.DtTimig.SelectedItem)?.DriverTiming;

        public SessionTiming SessionTiming
        {
            get => _timing;
            private set
            {
                _timing = value;
                NotifyPropertyChanged();
            }
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

        public string CurrentGear
        {
            get => (string)GetValue(CurrentGearProperty);
            set => SetValue(CurrentGearProperty, value);
        }

        public ICollectionView TimingInfo => ViewSource?.View;

        public bool IsDaemon => false;

        public void RunPlugin()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(RunPlugin);
                return;
            }

            ConnectedSource = "Not Connected";
            CreateDisplaySettings();
            CreateGuiInstance();
            CreateAutoSaver();

            ScheduleRefreshActions();

            OnDisplaySettingsChange(this, null);
            _shouldReset = ResetModeEnum.NoReset;
        }

        private void CreateAutoSaver()
        {
            _settingAutoSaver = new DisplaySettingAutoSaver(SettingsPath);
            _settingAutoSaver.DisplaySettingsModelView = DisplaySettings;
        }

        private bool TerminatePeriodicTasks { get; set; }

        private void ScheduleRefreshActions()
        {
            SchedulePeriodicAction(() => RefreshGui(_lastDataSet), 10000, this);
            SchedulePeriodicAction(() => RefreshBasicInfo(_lastDataSet), 33, this);
            SchedulePeriodicAction(() => RefreshTimingCircle(_lastDataSet), 300, this);
        }

        private void CreateGuiInstance()
        {
            Gui = new TimingGui();
            Gui.Show();
            Gui.Closed += Gui_Closed;
            Gui.MouseLeave += GuiOnMouseLeave;
            Gui.DataContext = this;
        }

        private void GuiOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            Gui.DtTimig.SelectedItem = null;
        }

        private void CreateDisplaySettings()
        {
            DisplaySettingsModelView displaySettingsModelView = new DisplaySettingsModelView();
            displaySettingsModelView.FromModel(
                new DisplaySettingsLoader().LoadDisplaySettingsFromFileSafe(SettingsPath));
            DisplaySettings = displaySettingsModelView;
            CurrentSessionOptions = SessionOptionsModelView.CreateFromModel(new SessionOptions());
        }

        public DisplaySettingsModelView DisplaySettings
        {
            get => (DisplaySettingsModelView) GetValue(DisplaySettingsProperty);
            set => SetValue(DisplaySettingsProperty, value);
        }

        public SessionOptionsModelView CurrentSessionOptions
        {
            get => (SessionOptionsModelView)GetValue(CurrentSessionOptionsProperty);
            set => SetValue(CurrentSessionOptionsProperty, value);
        }

        private ICommand _resetCommand;

        public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new NoArgumentCommand(ScheduleReset));

        public ICommand OpenSettingsCommand => new NoArgumentCommand(OpenSettingsWindow);

        public ICommand RightClickCommand => new NoArgumentCommand(UnSelectItem);

        public ICommand DoubleLeftClickCommand
        {
            get;
            set;
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
                DataContext = DisplaySettings,
                Owner = Gui
            };
            _settingsWindow.Show();
        }

        public int SessionCompletedPercentage => _timing?.SessionCompletedPerMiles ?? 50;

        private void PaceLapsChanged()
        {
            if (_timing != null)
            {
                _timing.PaceLaps = DisplaySettings.PaceLaps;
            }

            if (!TerminatePeriodicTasks)
            {
                Gui.Dispatcher.Invoke(RefreshDataGrid);
            }

        }

        private void ScheduleReset()
        {
            _shouldReset = ResetModeEnum.Manual;
        }

        private void ChangeOrderingMode()
        {
            if (ViewSource == null || _timing == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ChangeOrderingMode);
                return;
            }

            var mode = GetOrderTypeFromSettings();
            if (mode == DisplayModeEnum.Absolute)
            {
                ViewSource.SortDescriptions.Clear();
                ViewSource.SortDescriptions.Add(new SortDescription("DriverTiming.Position", ListSortDirection.Ascending));
                _timing.DisplayGapToPlayerRelative = false;
            }
            else
            {
                ViewSource.SortDescriptions.Clear();
                ViewSource.SortDescriptions.Add(new SortDescription("DriverTiming.DistanceToPlayer", ListSortDirection.Ascending));
                _timing.DisplayGapToPlayerRelative = true;
            }

        }

        private void ChangeTimeDisplayMode()
        {
            if (_timing == null || Gui == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ChangeTimeDisplayMode);
                return;
            }

            var mode = GetTimeDisplayTypeFromSettings();
            _timing.DisplayBindTimeRelative = mode == DisplayModeEnum.Relative;
            _timing.DisplayGapToPlayerRelative = mode == DisplayModeEnum.Relative;
        }

        private DisplayModeEnum GetOrderTypeFromSettings()
        {
            return CurrentSessionOptions.OrderingMode;
        }

        private DisplayModeEnum GetTimeDisplayTypeFromSettings()
        {
            return CurrentSessionOptions.TimesDisplayMode;
        }

        private SessionOptionsModelView GetSessionOptionOfCurrentSession(SimulatorDataSet dataSet)
        {
            if (dataSet == null)
            {
                return new SessionOptionsModelView();
            }

            switch (dataSet.SessionInfo.SessionType)
            {
                case SessionType.Practice:
                case SessionType.WarmUp:
                    return DisplaySettings.PracticeSessionDisplayOptions;
                case SessionType.Qualification:
                    return DisplaySettings.QualificationSessionDisplayOptions;
                case SessionType.Race:
                    return DisplaySettings.RaceSessionDisplayOptions;
                default:
                    return new SessionOptionsModelView();
            }
        }

        private void Gui_Closed(object sender, EventArgs e)
        {
            TerminatePeriodicTasks = true;
            _pluginsManager.DeletePlugin(this);
        }

        private void OnDataLoaded(object sender, DataEventArgs args)
        {
            if (Dispatcher.CheckAccess())
            {

                if (_lastDataSet?.SessionInfo.TrackInfo.TrackName != args.Data.SessionInfo.TrackInfo.TrackName)
                {
                    RefreshTrackInfo(args.Data);
                }

                _lastDataSet = args.Data;
                ConnectedSource = _lastDataSet.Source;
                if (ViewSource == null || _timing == null)
                {
                    return;
                }
                SimulatorDataSet data = args.Data;

                if (_sessionType != data.SessionInfo.SessionType)
                {
                    _shouldReset = ResetModeEnum.Automatic;
                    _sessionType = _timing.SessionType;
                }

                // Reset state was detected (either reset button was pressed or timing detected a session change)
                if (_shouldReset != ResetModeEnum.NoReset)
                {
                    CreateTiming(data);
                    _shouldReset = ResetModeEnum.NoReset;
                }

                try
                {
                    _timing?.UpdateTiming(data);
                    CurrentGear = data.PlayerInfo?.CarInfo?.CurrentGear;
                }
                catch (SessionTiming.DriverNotFoundException)
                {
                    _shouldReset = ResetModeEnum.Automatic;
                    return;
                }
            }
            else
            {
                Dispatcher.Invoke(() => OnDataLoaded(sender, args));
            }
        }

        private void RefreshTimingCircle(SimulatorDataSet data)
        {
            if (data == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => RefreshTimingCircle(data));
                return;
            }

            Gui.TimingCircle.RefreshSession(data);
        }

        private void RefreshBasicInfo(SimulatorDataSet data)
        {
            if (data == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => RefreshBasicInfo(data));
                return;
            }

            NotifyPropertyChanged("SessionTime");
            NotifyPropertyChanged("SystemTime");
            NotifyPropertyChanged("SessionCompletedPercentage");
            Gui.PedalControl.UpdateControl(data);
            Gui.WhLeftFront.UpdateControl(data);
            Gui.WhRightFront.UpdateControl(data);
            Gui.WhLeftRear.UpdateControl(data);
            Gui.WhRightRear.UpdateControl(data);
            Gui.FuelMonitor.ProcessDataSet(data);
            Gui.WaterTemp.Temperature = data.PlayerInfo.CarInfo.WaterSystemInfo.WaterTemperature;
            Gui.OilTemp.Temperature = data.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature;

            Gui.LblWeather.Content = GetWeatherInfo(data);
            SessionInfoViewModel.SessionRemaining = GetSessionRemaining(data);
        }

        private string GetWeatherInfo(SimulatorDataSet set)
        {
            WeatherInfo weather = set.SessionInfo.WeatherInfo;
            StringBuilder sb = new StringBuilder();

            if (weather.AirTemperature != Temperature.Zero)
            {
                sb.Append("Air: " + weather.AirTemperature.GetValueInUnits(DisplaySettings.TemperatureUnits)
                    .ToString("n1"));
            }

            if (weather.TrackTemperature != Temperature.Zero)
            {
                sb.Append(" |Track: " +weather.TrackTemperature.GetValueInUnits(DisplaySettings.TemperatureUnits)
                    .ToString("n1"));
            }


            if (weather.RainIntensity > 0)
            {
                sb.Append(" |Rain Intensity: " + weather.RainIntensity+"%");
            }

            return sb.ToString();

        }

        private void Timing_DriverRemoved(object sender, DriverListModificationEventArgs e)
        {
            if (TerminatePeriodicTasks)
            {
                return;
            }

            Gui?.Dispatcher.Invoke(() =>
            {
                Gui.TimingCircle.RemoveDriver(e.Data.DriverTiming.DriverInfo);
                Collection?.Remove(e.Data);
            });

        }

        private void Timing_DriverAdded(object sender, DriverListModificationEventArgs e)
        {
            Gui?.Dispatcher.Invoke(() =>
            {
                Collection?.Add(e.Data);
                Gui.TimingCircle.AddDriver(e.Data.DriverTiming.DriverInfo);
            });
            _driverLapsWindowManager.Rebind(e.Data.DriverTiming);
        }

        private void RefreshGui(SimulatorDataSet data)
        {
            if (data == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => RefreshGui(data));
                return;
            }

            Gui.PedalControl.UpdateControl(data);
            Gui.WhLeftFront.UpdateControl(data);
            Gui.WhRightFront.UpdateControl(data);
            Gui.WhLeftRear.UpdateControl(data);
            Gui.WhRightRear.UpdateControl(data);
            Gui.TimingCircle.RefreshSession(data);
            RefreshDataGrid();

            if (DisplaySettings.ScrollToPlayer && Gui != null && _timing?.Player != null && Gui.DtTimig.Items.Count > 0)
            {
                Gui.DtTimig.ScrollIntoView(Gui.DtTimig.Items[0]);
                Gui.DtTimig.ScrollIntoView(_timing.Player);
            }

        }

        private void RefreshDataGrid()
        {
            ViewSource?.View.Refresh();
        }

        private string GetSessionRemaining(SimulatorDataSet dataSet)
        {
            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Na)
            {
                return "NA";
            }

            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Time)
            {
                string timeRemaining = "Time Remaining: " + ((int)(dataSet.SessionInfo.SessionTimeRemaining / 60)) + ":"
                       + ((int)dataSet.SessionInfo.SessionTimeRemaining % 60).ToString("00");
                if (_timing?.Leader != null && dataSet.SessionInfo?.SessionType == SessionType.Race && _timing?.Leader?.DriverTiming?.Pace != TimeSpan.Zero)
                {
                    double lapsToGo = dataSet.SessionInfo.SessionTimeRemaining /
                                      _timing.Leader.DriverTiming.Pace.TotalSeconds;
                    timeRemaining += "\nLaps:" + lapsToGo.ToString("N1");
                }

                return timeRemaining;
            }

            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Laps)
            {
                int lapsToGo = dataSet.SessionInfo.TotalNumberOfLaps - dataSet.SessionInfo.LeaderCurrentLap + 1;
                if (lapsToGo < 1)
                {
                    return "Leader Finished";
                }
                if (lapsToGo == 1)
                {
                    return "Leader on Final Lap";
                }
                string lapsToDisplay = lapsToGo < 2000
                                           ? lapsToGo.ToString()
                                           : "Infinite";
                return "Leader laps to go: " + lapsToDisplay;
            }

            return "NA";
        }

        private void CreateTiming(SimulatorDataSet data)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => CreateTiming(data));
                return;
            }

            var invalidateLap = _shouldReset == ResetModeEnum.Manual ||
                                data.SessionInfo.SessionType != SessionType.Race;
            _lastDataSet = data;
            if (_timing != null && _reportCreation != null)
            {
                _reportCreation.CreateReport(_timing);
            }

            SessionTiming = SessionTiming.FromSimulatorData(data, invalidateLap, this);
            foreach (var driverTimingModelView in _timing.Drivers.Values)
            {
                _driverLapsWindowManager.Rebind(driverTimingModelView.DriverTiming);
            }
            SessionInfoViewModel.SessionTiming = _timing;
            _timing.DriverAdded += Timing_DriverAdded;
            _timing.DriverRemoved += Timing_DriverRemoved;
            _timing.PaceLaps = DisplaySettings.PaceLaps;

            InitializeGui(data);
            ChangeTimeDisplayMode();
            ChangeOrderingMode();
            ConnectedSource = data.Source;
            NotifyPropertyChanged("BestLapFormatted");
            NotifyPropertyChanged("ConnectedSource");
        }

        private void OnSessionStarted(object sender, DataEventArgs args)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => OnSessionStarted(sender, args));
                return;
            }

            CreateTiming(args.Data);
            UpdateCurrentSessionOption(args.Data);
        }

        private void UpdateCurrentSessionOption(SimulatorDataSet data)
        {
            CurrentSessionOptions = GetSessionOptionOfCurrentSession(data);
        }

        private void InitializeGui(SimulatorDataSet data)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => InitializeGui(data));
                return;
            }

            if (ViewSource == null)
            {
                ViewSource = new CollectionViewSource { Source = Collection };
                Gui.DtTimig.DataContext = null;
                Gui.DtTimig.DataContext = this;
            }

            Collection.Clear();
            foreach (DriverTimingModelView d in _timing.Drivers.Values)
            {
                Collection.Add(d);
            }

            RefreshTrackInfo(data);

            Gui.TimingCircle.SetSessionInfo(data);
            Gui.FuelMonitor.ResetFuelMonitor();

            NotifyPropertyChanged("BestLapFormatted");
        }

        private void RefreshTrackInfo(SimulatorDataSet data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(data.SessionInfo.TrackInfo.TrackName);
            if (!string.IsNullOrWhiteSpace(data.SessionInfo.TrackInfo.TrackLayoutName))
            {
                sb.Append(" (");
                sb.Append(data.SessionInfo.TrackInfo.TrackLayoutName);

                sb.Append(") ");
            }
            sb.Append(" - ");
            sb.Append(data.SessionInfo.SessionType);
            Gui.LblTrack.Content = sb.ToString();
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnDisplaySettingsChange(object sender, PropertyChangedEventArgs args)
        {
            ApplyDisplaySettings(DisplaySettings);
            if (args?.PropertyName == "PaceLaps")
            {
                PaceLapsChanged();
            }

            if (args?.PropertyName == SessionOptionsModelView.OrderingModeProperty.Name)
            {
                ChangeOrderingMode();
            }

            if (args?.PropertyName == SessionOptionsModelView.TimesDisplayModeProperty.Name)
            {
                ChangeTimeDisplayMode();
            }

        }

        private void ApplyDisplaySettings(DisplaySettingsModelView settings)
        {
            if (settings == null || Gui == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => ApplyDisplaySettings(settings));
                return;
            }

            Gui.WhLeftFront.TemperatureDisplayUnit = settings.TemperatureUnits;
            Gui.WhRightFront.TemperatureDisplayUnit = settings.TemperatureUnits;
            Gui.WhLeftRear.TemperatureDisplayUnit = settings.TemperatureUnits;
            Gui.WhRightRear.TemperatureDisplayUnit = settings.TemperatureUnits;

            Gui.WhLeftFront.PressureDisplayUnits = settings.PressureUnits;
            Gui.WhRightFront.PressureDisplayUnits = settings.PressureUnits;
            Gui.WhLeftRear.PressureDisplayUnits = settings.PressureUnits;
            Gui.WhRightRear.PressureDisplayUnits = settings.PressureUnits;

        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            TimingDataViewModel timingDataViewModel = (TimingDataViewModel) dependencyObject;
            DisplaySettingsModelView newDisplaySettingsModelView =
                (DisplaySettingsModelView) dependencyPropertyChangedEventArgs.NewValue;
            newDisplaySettingsModelView.PropertyChanged += timingDataViewModel.OnDisplaySettingsChange;
            newDisplaySettingsModelView.PracticeSessionDisplayOptions.PropertyChanged += timingDataViewModel.OnDisplaySettingsChange;
            newDisplaySettingsModelView.RaceSessionDisplayOptions.PropertyChanged += timingDataViewModel.OnDisplaySettingsChange;
            newDisplaySettingsModelView.QualificationSessionDisplayOptions.PropertyChanged += timingDataViewModel.OnDisplaySettingsChange;

            if (timingDataViewModel._settingAutoSaver != null)
            {
                timingDataViewModel._settingAutoSaver.DisplaySettingsModelView = newDisplaySettingsModelView;
            }

            if (timingDataViewModel._reportCreation != null)
            {
                timingDataViewModel._reportCreation.Settings = newDisplaySettingsModelView;
            }

        }

        private static async void SchedulePeriodicAction(Action action, int periodInMs, TimingDataViewModel sender)
        {
            try
            {
                while (!sender.TerminatePeriodicTasks)
                {
                    await Task.Delay(periodInMs, CancellationToken.None);

                    if (!sender.TerminatePeriodicTasks)
                    {
                        action();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private static void CurrentSessionOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TimingDataViewModel timingDataViewModel)
            {
                timingDataViewModel.ChangeOrderingMode();
                timingDataViewModel.ChangeTimeDisplayMode();
            }
        }

        private void UnSelectItem()
        {
            if (Gui == null)
            {
                return;
            }

            Gui.DtTimig.SelectedItem = null;
        }


        private static void DisplayMessage(object sender, MessageArgs e)
        {
            if (e.IsDecision)
            {
                if (MessageBox.Show(e.Message, "Message from connector.", MessageBoxButton.YesNo,
                        MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    e.Action();
                }
            }
            else
            {
                MessageBox.Show(e.Message, "Message from connector.", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

    }
}
