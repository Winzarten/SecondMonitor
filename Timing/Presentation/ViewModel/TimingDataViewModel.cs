namespace SecondMonitor.Timing.Presentation.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using SecondMonitor.DataModel;
    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.PluginManager.Core;
    using SecondMonitor.PluginManager.GameConnector;
    using SecondMonitor.Timing.LapTimings.ViewModel;
    using SecondMonitor.Timing.Presentation.View;
    using SecondMonitor.Timing.Presentation.ViewModel.Commands;
    using SecondMonitor.Timing.SessionTiming.Drivers;
    using SecondMonitor.Timing.SessionTiming.Drivers.ModelView;
    using SecondMonitor.Timing.SessionTiming.Drivers.Presentation.ViewModel;
    using SecondMonitor.Timing.SessionTiming.ViewModel;
    using SecondMonitor.Timing.Settings;
    using SecondMonitor.Timing.Settings.Model;
    using SecondMonitor.Timing.Settings.ModelView;

    using DriverLapsWindow = SecondMonitor.Timing.LapTimings.View.DriverLapsWindow;

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
        private PluginsManager _pluginsManager;
        private SessionTiming _timing;
        private DisplaySettingsWindow _settingsWindow;
        private SessionType _sessionType = SessionType.Na;
        private SimulatorDataSet _lastDataSet;
        private readonly DriverLapsWindowManager _driverLapsWindowManager;

        public TimingDataViewModel()
        {
            SessionInfoViewModel = new SessionInfoViewModel();
            _driverLapsWindowManager = new DriverLapsWindowManager(() => Gui, () => SelectedDriverTiming);
            DoubleLeftClickCommand = this._driverLapsWindowManager.OpenWindowCommand;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Gets or sets the CollectionViewSource
        public CollectionViewSource ViewSource { get; set; } 

        // Gets or sets the ObservableCollection
        public ObservableCollection<DriverTimingModelView> Collection { get; set; } = new ObservableCollection<DriverTimingModelView>();

        public string SessionTime => _timing?.SessionTime.ToString("mm\\:ss\\.fff") ?? string.Empty;

        public string ConnectedSource { get; private set; }

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

        public PluginsManager PluginManager
        {
            get => _pluginsManager;
            set
            {
                _pluginsManager = value;
                _pluginsManager.DataLoaded += OnDataLoaded;
                _pluginsManager.SessionStarted += OnSessionStarted;
            }
        }

        public string CurrentGear
        {
            get => (string)GetValue(CurrentGearProperty);
            set => SetValue(CurrentGearProperty, value);
        }

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

        private void ScheduleRefreshActions()
        {
            SchedulePeriodicAction(() => RefreshGui(_lastDataSet), 10000, this, CancellationToken.None);
            SchedulePeriodicAction(() => RefreshBasicInfo(_lastDataSet), 33, this, CancellationToken.None);
            SchedulePeriodicAction(() => RefreshTimingCircle(_lastDataSet), 300, this, CancellationToken.None);
        }

        private void CreateGuiInstance()
        {
            this.Gui = new TimingGui();
            this.Gui.Show();
            this.Gui.Closed += Gui_Closed;
            this.Gui.MouseLeave += GuiOnMouseLeave;
            this.Gui.DataContext = this;
        }

        private void GuiOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            this.Gui.DtTimig.SelectedItem = null;
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
                Owner = this.Gui
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
            
            this.Gui.Dispatcher.Invoke(RefreshDataGrid);

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
            if (_timing == null || this.Gui == null)
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
            _pluginsManager.DeletePlugin(this);
        }

        private void OnDataLoaded(object sender, DataEventArgs args)
        {
            _lastDataSet = args.Data;
            if (ViewSource == null || _timing == null)
            {
                return;
            }

            if (Dispatcher.CheckAccess())
            {
                SimulatorDataSet data = args.Data;

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

                if (_sessionType != _timing.SessionType)
                {
                    _shouldReset = ResetModeEnum.Automatic;
                    _sessionType = _timing.SessionType;
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

            this.Gui.TimingCircle.RefreshSession(data);
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
            this.Gui.PedalControl.UpdateControl(data);
            this.Gui.WhLeftFront.UpdateControl(data);
            this.Gui.WhRightFront.UpdateControl(data);
            this.Gui.WhLeftRear.UpdateControl(data);
            this.Gui.WhRightRear.UpdateControl(data);
            this.Gui.FuelMonitor.ProcessDataSet(data);
            this.Gui.WaterTemp.Temperature = data.PlayerInfo.CarInfo.WaterSystemInfo.WaterTemperature;
            this.Gui.OilTemp.Temperature = data.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature;

            this.Gui.LblWeather.Content =
                "Air: "
                + data.SessionInfo.WeatherInfo.AirTemperature.GetValueInUnits(DisplaySettings.TemperatureUnits)
                    .ToString("n1") + Temperature.GetUnitSymbol(DisplaySettings.TemperatureUnits) + " |Track: "
                + data.SessionInfo.WeatherInfo.TrackTemperature.GetValueInUnits(DisplaySettings.TemperatureUnits).ToString("n1")
                + Temperature.GetUnitSymbol(DisplaySettings.TemperatureUnits);
            SessionInfoViewModel.SessionRemaining = GetSessionRemaining(data);
        }

        private void Timing_DriverRemoved(object sender, DriverListModificationEventArgs e)
        {
            this.Gui?.Dispatcher.Invoke(() =>
            {
                this.Gui.TimingCircle.RemoveDriver(e.Data.DriverTiming.DriverInfo);
                Collection?.Remove(e.Data);
            });

        }

        private void Timing_DriverAdded(object sender, DriverListModificationEventArgs e)
        {
            this.Gui?.Dispatcher.Invoke(() =>
            {
                Collection?.Add(e.Data);
                this.Gui.TimingCircle.AddDriver(e.Data.DriverTiming.DriverInfo);
            });
            this._driverLapsWindowManager.Rebind(e.Data.DriverTiming);
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

            this.Gui.PedalControl.UpdateControl(data);
            this.Gui.WhLeftFront.UpdateControl(data);
            this.Gui.WhRightFront.UpdateControl(data);
            this.Gui.WhLeftRear.UpdateControl(data);
            this.Gui.WhRightRear.UpdateControl(data);
            this.Gui.TimingCircle.RefreshSession(data);
            RefreshDataGrid();
            if (DisplaySettings.ScrollToPlayer && this.Gui != null && _timing?.Player != null && this.Gui.DtTimig.Items.Count > 0)
            {
                this.Gui.DtTimig.ScrollIntoView(this.Gui.DtTimig.Items[0]);
                this.Gui.DtTimig.ScrollIntoView(_timing.Player);
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
                return "Time Remaining: " + ((int)(dataSet.SessionInfo.SessionTimeRemaining / 60)) + ":"
                       + ((int)dataSet.SessionInfo.SessionTimeRemaining % 60).ToString("00");
            }

            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Laps)
            {
                return "Laps: " + (dataSet.SessionInfo.LeaderCurrentLap + "/" + dataSet.SessionInfo.TotalNumberOfLaps);
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

            _timing = SessionTiming.FromSimulatorData(data, invalidateLap, this);
            foreach (var driverTimingModelView in this._timing.Drivers.Values)
            {
                this._driverLapsWindowManager.Rebind(driverTimingModelView.DriverTiming);
            }
            SessionInfoViewModel.SessionTiming = this._timing;
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

        public ICollectionView TimingInfo { get => ViewSource!= null ? ViewSource.View : null; }

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
                this.Gui.DtTimig.DataContext = null;
                this.Gui.DtTimig.DataContext = this;
            }

            Collection.Clear();
            foreach (DriverTimingModelView d in _timing.Drivers.Values)
            {
                Collection.Add(d);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(data.SessionInfo.TrackInfo.TrackName);
            sb.Append(" (");
            sb.Append(data.SessionInfo.TrackInfo.TrackLayoutName);

            sb.Append(") - ");
            sb.Append(data.SessionInfo.SessionType);
            this.Gui.LblTrack.Content = sb.ToString();

            this.Gui.TimingCircle.SetSessionInfo(data);
            this.Gui.FuelMonitor.ResetFuelMonitor();

            NotifyPropertyChanged("BestLapFormatted");
        }


        protected virtual void NotifyPropertyChanged(string propertyName)
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
            if (settings == null || this.Gui == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => ApplyDisplaySettings(settings));
                return;
            }

            this.Gui.WhLeftFront.TemperatureDisplayUnit = settings.TemperatureUnits;
            this.Gui.WhRightFront.TemperatureDisplayUnit = settings.TemperatureUnits;
            this.Gui.WhLeftRear.TemperatureDisplayUnit = settings.TemperatureUnits;
            this.Gui.WhRightRear.TemperatureDisplayUnit = settings.TemperatureUnits;

            this.Gui.WhLeftFront.PressureDisplayUnits = settings.PressureUnits;
            this.Gui.WhRightFront.PressureDisplayUnits = settings.PressureUnits;
            this.Gui.WhLeftRear.PressureDisplayUnits = settings.PressureUnits;
            this.Gui.WhRightRear.PressureDisplayUnits = settings.PressureUnits;

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

        }

        private static async void SchedulePeriodicAction(Action action, int periodInMs, object sender, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(periodInMs, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    action();
                }
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
            if (this.Gui == null)
            {
                return;
            }

            this.Gui.DtTimig.SelectedItem = null;
        }

    }
}
