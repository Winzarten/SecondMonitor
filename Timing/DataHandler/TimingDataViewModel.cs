namespace SecondMonitor.Timing.DataHandler
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

    using SecondMonitor.DataModel;
    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.PluginManager.Core;
    using SecondMonitor.PluginManager.GameConnector;
    using SecondMonitor.Timing.DataHandler.Commands;
    using SecondMonitor.Timing.GUI;
    using SecondMonitor.Timing.Model;
    using SecondMonitor.Timing.Model.Drivers;
    using SecondMonitor.Timing.Model.Drivers.ModelView;
    using SecondMonitor.Timing.Model.Settings.Model;
    using SecondMonitor.Timing.Model.Settings.ModelView;
    using SecondMonitor.Timing.Settings;
    using SecondMonitor.Timing.Settings.ModelView;

    public class TimingDataViewModel : DependencyObject, ISecondMonitorPlugin, INotifyPropertyChanged
    {

        public static readonly DependencyProperty DisplaySettingsProperty = DependencyProperty.Register("DisplaySettings", typeof(DisplaySettingsModelView), typeof(TimingDataViewModel), new PropertyMetadata(null, PropertyChangedCallback));
        public static readonly DependencyProperty CurrentSessionOptionsProperty = DependencyProperty.Register("CurrentSessionOptions", typeof(SessionOptionsModelView), typeof(TimingDataViewModel), new PropertyMetadata(null, CurrentSessionOptionsPropertyChanged));

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
        private TimingGui _gui;
        private DisplaySettingAutoSaver _settingAutoSaver;
        private PluginsManager _pluginsManager;
        private SessionTiming _timing;
        private DisplaySettingsWindow _settingsWindow;
        private SessionInfo.SessionTypeEnum _sessionType = SessionInfo.SessionTypeEnum.Na;        
        

        public event PropertyChangedEventHandler PropertyChanged;

        private SimulatorDataSet _lastDataSet;
      
        // Gets or sets the CollectionViewSource
        public CollectionViewSource ViewSource { get; set; } 

        // Gets or sets the ObservableCollection
        public ObservableCollection<DriverTimingModelView> Collection { get; set; } = new ObservableCollection<DriverTimingModelView>();

        private LapInfo _bestSessionLap;

        public string BestLapFormatted => _bestSessionLap != null
                                              ? _bestSessionLap.Driver.DriverInfo.DriverName + "-(L" + _bestSessionLap.LapNumber + "):"
                                                + DriverTiming.FormatTimeSpan(_bestSessionLap.LapTime)
                                              : "Best Session Lap";

        public string SessionTime => _timing?.SessionTime.ToString("mm\\:ss\\.fff") ?? string.Empty;

        public string ConnectedSource { get; private set; }

        public string SystemTime => DateTime.Now.ToString("HH:mm");

        public static string Title
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string version = AssemblyName.GetAssemblyName(assembly.Location).Version.ToString();
                return "Second Monitor (Timing)(v" + version + " )";
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
            }
        }

        public bool IsDaemon => false;

        public void RunPlugin()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(this.RunPlugin);
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
            _gui = new TimingGui();
            _gui.Show();
            _gui.Closed += Gui_Closed;
            _gui.DataContext = this;
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

        private NoArgumentCommand _resetCommand;

        public NoArgumentCommand ResetCommand => this._resetCommand ?? (this._resetCommand = new NoArgumentCommand(this.ScheduleReset));

        public NoArgumentCommand OpenSettingsCommand => new NoArgumentCommand(OpenSettingsWindow);

        public NoArgumentCommand RightClickCommand => new NoArgumentCommand(this.UnSelectItem);


        private void OpenSettingsWindow()
        {
            if (_settingsWindow != null && _settingsWindow.IsVisible)
            {
                _settingsWindow.Focus();
                return;
            }

            _settingsWindow = new DisplaySettingsWindow();
            _settingsWindow.DataContext = DisplaySettings;
            this._settingsWindow.Owner = this._gui;
            _settingsWindow.Show();
        }

        public int SessionCompletedPercentage => _timing?.SessionCompletedPerMiles ?? 50;

        private void PaceLapsChanged()
        {
            if (_timing != null)
            {
                _timing.PaceLaps = DisplaySettings.PaceLaps;
            }
            
            _gui.Dispatcher.Invoke(RefreshDataGrid);

        }

        private void ScheduleReset()
        {
            _shouldReset = ResetModeEnum.Manual;
        }

        private void ChangeOrderingMode()
        {
            if (ViewSource == null || this._timing == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(this.ChangeOrderingMode);
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
            if (_timing == null || _gui == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(this.ChangeTimeDisplayMode);
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
                case SessionInfo.SessionTypeEnum.Practice:
                case SessionInfo.SessionTypeEnum.WarmUp:
                    return DisplaySettings.PracticeSessionDisplayOptions;
                case SessionInfo.SessionTypeEnum.Qualification:
                    return DisplaySettings.QualificationSessionDisplayOptions;
                case SessionInfo.SessionTypeEnum.Race:
                    return DisplaySettings.RaceSessionDisplayOptions;
                case SessionInfo.SessionTypeEnum.Na:
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
            if (ViewSource == null || this._timing == null)
            {
                return;
            }

            if (this.Dispatcher.CheckAccess())
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
                this.Dispatcher.Invoke(() => OnDataLoaded(sender, args));
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

            _gui.TimingCircle.RefreshSession(data);
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
            NotifyPropertyChanged("SessionCompletedPerMiles");
            _gui.PedalControl.UpdateControl(data);
            _gui.WhLeftFront.UpdateControl(data);
            _gui.WhRightFront.UpdateControl(data);
            _gui.WhLeftRear.UpdateControl(data);
            _gui.WhRightRear.UpdateControl(data);
            _gui.FuelMonitor.ProcessDataSet(data);
            _gui.WaterTemp.Temperature = data.PlayerInfo.CarInfo.WaterSystmeInfo.WaterTemperature;
            _gui.OilTemp.Temperature = data.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature;

            _gui.LblWeather.Content =
                "Air: "
                + data.SessionInfo.WeatherInfo.AirTemperature.GetValueInUnits(DisplaySettings.TemperatureUnits)
                    .ToString("n1") + Temperature.GetUnitSymbol(DisplaySettings.TemperatureUnits) + " |Track: "
                + data.SessionInfo.WeatherInfo.TrackTemperature.GetValueInUnits(DisplaySettings.TemperatureUnits).ToString("n1")
                + Temperature.GetUnitSymbol(DisplaySettings.TemperatureUnits);
            _gui.LblRemainig.Content = this.GetSessionRemaining(data);
        }

        private void Timing_DriverRemoved(object sender, DriverListModificationEventArgs e)
        {
            _gui?.Dispatcher.Invoke(() =>
            {
                _gui.TimingCircle.RemoveDriver(e.Data.DriverTiming.DriverInfo);
                Collection?.Remove(e.Data);
            });

        }

        private void Timing_DriverAdded(object sender, DriverListModificationEventArgs e)
        {
            _gui?.Dispatcher.Invoke(() =>
            {
                Collection?.Add(e.Data);
                _gui.TimingCircle.AddDriver(e.Data.DriverTiming.DriverInfo);
            });
        }

        private void RefreshGui(SimulatorDataSet data)
        {
            if (data == null)
            {
                return;
            }
            if (!this.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => RefreshGui(data));
                return;
            }

            _gui.PedalControl.UpdateControl(data);
            _gui.WhLeftFront.UpdateControl(data);
            _gui.WhRightFront.UpdateControl(data);
            _gui.WhLeftRear.UpdateControl(data);
            _gui.WhRightRear.UpdateControl(data);
            _gui.TimingCircle.RefreshSession(data);
            RefreshDataGrid();
            if (DisplaySettings.ScrollToPlayer && _gui != null && _timing?.Player != null && _gui.DtTimig.Items.Count > 0)
            {
                _gui.DtTimig.ScrollIntoView(_gui.DtTimig.Items[0]);
                _gui.DtTimig.ScrollIntoView(_timing.Player);
            }

        }

        private void RefreshDataGrid()
        {
            this.ViewSource?.View.Refresh();
        }

        private string GetSessionRemaining(SimulatorDataSet dataSet)
        {
            if (dataSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.Na)
            {
                return "NA";
            }

            if (dataSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.Time)
            {
                return "Time Remaining: " + ((int)(dataSet.SessionInfo.SessionTimeRemaining / 60)) + ":"
                       + ((int)dataSet.SessionInfo.SessionTimeRemaining % 60).ToString("00");
            }

            if (dataSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.Laps)
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
                                data.SessionInfo.SessionType != SessionInfo.SessionTypeEnum.Race;
            this._lastDataSet = data;

            _timing = SessionTiming.FromSimulatorData(data, invalidateLap, this);
            _timing.BestLapChangedEvent += BestLapChangedHandler;
            _timing.DriverAdded += Timing_DriverAdded;
            _timing.DriverRemoved += Timing_DriverRemoved;
            _timing.PaceLaps = DisplaySettings.PaceLaps;

            InitializeGui(data);
            this.ChangeTimeDisplayMode();
            this.ChangeOrderingMode();
            ConnectedSource = data.Source;
            _bestSessionLap = null;
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
                ViewSource = new CollectionViewSource { Source = this.Collection };
                _gui.DtTimig.DataContext = null;
                _gui.DtTimig.DataContext = this;                
            }

            Collection.Clear();
            foreach (DriverTimingModelView d in _timing.Drivers.Values)
            {
                Collection.Add(d);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(data.SessionInfo.TrackName);
            sb.Append(" (");
            sb.Append(data.SessionInfo.TrackLayoutName);

            sb.Append(") - ");
            sb.Append(data.SessionInfo.SessionType);
            _gui.LblTrack.Content = sb.ToString();

            _gui.TimingCircle.SetSessionInfo(data);
            _gui.FuelMonitor.ResetFuelMonitor();

            NotifyPropertyChanged("BestLapFormatted");
        }

        private void BestLapChangedHandler(object sender, SessionTiming.BestLapChangedArgs args)
        {
            _bestSessionLap = args.Lap;
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
                this.ChangeOrderingMode();
            }

            if (args?.PropertyName == SessionOptionsModelView.TimesDisplayModeProperty.Name)
            {
                this.ChangeTimeDisplayMode();
            }

        }

        private void ApplyDisplaySettings(DisplaySettingsModelView settings)
        {
            if (settings == null || this._gui == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => ApplyDisplaySettings(settings));
                return;
            }

            _gui.WhLeftFront.TemperatureDisplayUnit = settings.TemperatureUnits;
            _gui.WhRightFront.TemperatureDisplayUnit = settings.TemperatureUnits;
            _gui.WhLeftRear.TemperatureDisplayUnit = settings.TemperatureUnits;
            _gui.WhRightRear.TemperatureDisplayUnit = settings.TemperatureUnits;

            _gui.WhLeftFront.PressureDisplayUnits = settings.PressureUnits;
            _gui.WhRightFront.PressureDisplayUnits = settings.PressureUnits;
            _gui.WhLeftRear.PressureDisplayUnits = settings.PressureUnits;
            _gui.WhRightRear.PressureDisplayUnits = settings.PressureUnits;

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
            if (this._gui == null)
            {
                return;
            }

            this._gui.DtTimig.SelectedItem = null;
        }

    }
}
