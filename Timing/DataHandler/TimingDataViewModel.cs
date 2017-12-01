using System;
using System.Text;
using SecondMonitor.PluginManager.Core;
using SecondMonitor.DataModel;
using SecondMonitor.PluginManager.GameConnector;
using SecondMonitor.Timing.Model;
using SecondMonitor.Timing.GUI;
using SecondMonitor.Timing.Model.Drivers;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using SecondMonitor.Timing.DataHandler.Commands;
using SecondMonitor.Timing.Model.Settings.Model;
using SecondMonitor.Timing.Model.Settings.ModelView;

namespace SecondMonitor.Timing.DataHandler
{

    public class TimingDataViewModel : DependencyObject, ISecondMonitorPlugin, INotifyPropertyChanged
    {

        public static readonly DependencyProperty DisplaySettingsProperty = DependencyProperty.Register("DisplaySettings", typeof(DisplaySettingsModelView), typeof(TimingDataViewModel), new PropertyMetadata(null, PropertyChangedCallback));


        private enum ResetModeEnum {  NoReset, Manual, Automatic}
        private TimingGui _gui;
        private PluginsManager _pluginsManager;
        private SessionTiming _timing;
        private SessionInfo.SessionTypeEnum _sessionType = SessionInfo.SessionTypeEnum.Na;        
        private bool _scrollToPlayer = true;
        private int _refreshRate = 1000;
        ResetModeEnum _shouldReset = ResetModeEnum.NoReset;
        public event PropertyChangedEventHandler PropertyChanged;


        private DateTime _lastRefreshTiming;
        private DateTime _lastRefreshCarInfo;
        private DateTime _lastRefreshCircleInfo;

        // Gets or sets the CollectionViewSource
        public CollectionViewSource ViewSource { get; set; } 

        // Gets or sets the ObservableCollection
        public ObservableCollection<DriverTiming> Collection { get; set; } = new ObservableCollection<DriverTiming>();

        private LapInfo _bestSessionLap;
        public string BestLapFormatted { get => _bestSessionLap != null ? _bestSessionLap.Driver.DriverInfo.DriverName +"-(L"+ _bestSessionLap.LapNumber+"):"+ DriverTiming.FormatTimeSpan(_bestSessionLap.LapTime) : "Best Session Lap"; }
        public string SessionTime { get => _timing != null ? _timing.SessionTime.ToString("mm\\:ss\\.fff") : ""; }

        public string ConnectedSource { get; private set; }
        public string SystemTime { get => DateTime.Now.ToString("HH:mm"); }

        public string Title
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                String version = AssemblyName.GetAssemblyName(assembly.Location).Version.ToString();
                return "Second Monitor (Timing)(v" + version +" )";
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
            ConnectedSource = "Not Connected";
            CreateDisplaySettings();
            _gui = new TimingGui();
            _gui.Show();            
            _gui.Closed += Gui_Closed;
            _gui.DataContext = this;
            OnDisplaySettingsChange(this, null);
            _lastRefreshTiming = DateTime.Now;
            _lastRefreshCarInfo = DateTime.Now;
            _shouldReset = ResetModeEnum.NoReset;
        }

        private void CreateDisplaySettings()
        {
            DisplaySettingsModelView displaySettingsModelView = new DisplaySettingsModelView();
            displaySettingsModelView.FromModel(new DisplaySettings());
            DisplaySettings = displaySettingsModelView;
        }

        public DisplaySettingsModelView DisplaySettings
        {
            get => (DisplaySettingsModelView) GetValue(DisplaySettingsProperty);
            set => SetValue(DisplaySettingsProperty, value);
        }

        private NoArgumentCommand _resetCommand;
        public NoArgumentCommand ResetCommand
        {
            get
            {
                if(_resetCommand==null)
                {
                    _resetCommand = new NoArgumentCommand(ScheduleReset);
                }
                return _resetCommand;
            }
        }
        
        private NoArgumentCommand _scrollToPlayerCommand;
        public NoArgumentCommand ScrollToPlayerCommand
        {
            get
            {
                if (_scrollToPlayerCommand == null)
                {
                    _scrollToPlayerCommand = new NoArgumentCommand(ScrollToPlayerChanged);
                }
                return _scrollToPlayerCommand;
            }
        }

        private TimingModeChangedCommand _timingModeChangedCommand;
        public TimingModeChangedCommand TimingModeChangedCommand
        {
            get
            {
                return _timingModeChangedCommand ?? (_timingModeChangedCommand =
                           new TimingModeChangedCommand(ChangeTimingMode, () => { return ViewSource != null; }));
            }
        }

        private TimingModeChangedCommand _timingDisplayModeChangedCommand;
        public TimingModeChangedCommand TimingDisplayModeChangedCommand
        {
            get
            {
                return _timingDisplayModeChangedCommand ?? (_timingDisplayModeChangedCommand =
                           new TimingModeChangedCommand(ChangeDisplayMode, () => { return _timing != null; }));
            }
        }

        public int SessionCompletedPercentage => _timing != null ? _timing.SessionCompletedPercentage : 50;

        private void ScrollToPlayerChanged()
        {
            _scrollToPlayer = (bool)_gui.chkScrollToPlayer.IsChecked;
        }

        private void PaceLapsChanged()
        {
            if (_timing != null)
                _timing.PaceLaps = DisplaySettings.PaceLaps;
            _gui.Dispatcher.Invoke(RefreshDatagrid);

        }        
        private void ScheduleReset()
        {
            _shouldReset = ResetModeEnum.Manual;
        }

        private void ChangeTimingMode()
        {
            _gui.Dispatcher.Invoke(() =>
            {
                var mode = _gui.TimingMode;
                if (mode == TimingGui.TimingModeOptions.Absolute || (mode == TimingGui.TimingModeOptions.Automatic && _timing.SessionType != SessionInfo.SessionTypeEnum.Race))
                {
                    ViewSource.SortDescriptions.Clear();
                    ViewSource.SortDescriptions.Add(new SortDescription("Position", ListSortDirection.Ascending));
                    _timing.DisplayGapToPlayerRelative = false;
                }
                else
                {
                    ViewSource.SortDescriptions.Clear();
                    ViewSource.SortDescriptions.Add(new SortDescription("DistanceToPlayer", ListSortDirection.Ascending));
                    _timing.DisplayGapToPlayerRelative = true;
                }
            });
        }

        private void ChangeDisplayMode()
        {
            if (_timing == null || _gui == null)
                return;
            _gui.Dispatcher.Invoke(() =>
            {
                var mode = _gui.TimingMode;
                _timing.DisplayBindTimeRelative = (bool)_gui.rbtTimeRelative.IsChecked;
                _timing.DisplayGapToPlayerRelative = !(mode == TimingGui.TimingModeOptions.Absolute || (mode == TimingGui.TimingModeOptions.Automatic && _timing.SessionType != SessionInfo.SessionTypeEnum.Race));
            });
        }

        private void Gui_Closed(object sender, EventArgs e)
        {
            _pluginsManager.DeletePlugin(this);
        }

        private void OnDataLoaded(object sender, DataEventArgs args)
        {
            SimulatorDataSet data = args.Data;
            if (ViewSource == null)
                return;
            //Reset state was detected (either reset button was pressed or timing detected a session change)
            if (_shouldReset != ResetModeEnum.NoReset)
            {
                CreateTiming(data);
                _shouldReset = ResetModeEnum.NoReset;
            }
            try
            {
                _timing?.UpdateTiming(data);
            }catch(SessionTiming.DriverNotFoundException)
            {
                _shouldReset = ResetModeEnum.Automatic;
                return;
            }
            var timeSpan = DateTime.Now.Subtract(_lastRefreshTiming);
           
            if(_sessionType!= _timing.SessionType)
            {
                _shouldReset = ResetModeEnum.Automatic;
                _sessionType = _timing.SessionType;
                return;
            }
            if (timeSpan.TotalMilliseconds > _refreshRate)
            {
                RefreshGui(data);
            }
            TimeSpan timeSpanCarIno = DateTime.Now.Subtract(_lastRefreshCarInfo);
            if (timeSpanCarIno.TotalMilliseconds > 33)
            {
                _gui.Dispatcher.Invoke((Action)(() =>
                {                    
                    NotifyPropertyChanged("SessionTime");
                    _gui.pedalControl.UpdateControl(data);
                    _gui.whLeftFront.UpdateControl(data);
                    _gui.whRightFront.UpdateControl(data);
                    _gui.whLeftRear.UpdateControl(data);
                    _gui.whRightRear.UpdateControl(data);
                    _gui.fuelMonitor.ProcessDataSet(data);
                }));
                _lastRefreshCarInfo = DateTime.Now;
            }
            timeSpanCarIno = DateTime.Now.Subtract(_lastRefreshCircleInfo);
            if (timeSpanCarIno.TotalMilliseconds > 200)
            {
                _gui.Dispatcher.Invoke((Action)(() =>
                {
                    _gui.timingCircle.RefreshSession(data);
                }));
                _lastRefreshCircleInfo = DateTime.Now;
            }
        }

        private void Timing_DriverRemoved(object sender, DriverListModificationEventArgs e)
        {
            _gui?.Dispatcher.Invoke(() =>
            {
                _gui.timingCircle.RemoveDriver(e.Data.DriverInfo);
                Collection?.Remove(e.Data);
            });

        }

        private void Timing_DriverAdded(object sender, DriverListModificationEventArgs e)
        {
            _gui?.Dispatcher.Invoke(() =>
            {
                Collection?.Add(e.Data);
                _gui.timingCircle.AddDriver(e.Data.DriverInfo);
            });
            

        }

        private void RefreshGui(SimulatorDataSet data)
        {
            _gui.Dispatcher.Invoke((Action)(() =>
            {
                NotifyPropertyChanged("SystemTime");
                NotifyPropertyChanged("SessionCompletedPercentage");
                _gui.pedalControl.UpdateControl(data);
                _gui.whLeftFront.UpdateControl(data);
                _gui.whRightFront.UpdateControl(data);
                _gui.whLeftRear.UpdateControl(data);
                _gui.whRightRear.UpdateControl(data);
                _gui.waterTemp.Temperature = data.PlayerInfo.CarInfo.WaterSystmeInfo.WaterTemperature;
                _gui.oilTemp.Temperature = data.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature;
                _gui.timingCircle.RefreshSession(data);


                _gui.lblWeather.Content = "Air: " + data.SessionInfo.WeatherInfo.AirTemperature.InCelsius.ToString("n1") + " |Track: " + data.SessionInfo.WeatherInfo.TrackTemperature.InCelsius.ToString("n1")
                + "| Rain Intensity: " + data.SessionInfo.WeatherInfo.RainIntensity + "%";
                _gui.lblRemainig.Content = GetSessionRemainig(data);
                RefreshDatagrid();
                if (_scrollToPlayer && _gui != null && _timing.Player != null && _gui.dtTimig.Items.Count > 0)
                {
                    _gui.dtTimig.ScrollIntoView(_gui.dtTimig.Items[0]);
                    _gui.dtTimig.ScrollIntoView(_timing.Player);
                }
            }));
            _lastRefreshTiming = DateTime.Now;
        }

        private void RefreshDatagrid()
        {
            if (ViewSource != null)
                ViewSource.View.Refresh();
        }

        private string GetSessionRemainig(SimulatorDataSet dataSet)
        {
            if (dataSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.Na)
                return "NA";
            if (dataSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.Time)
                return "Time Remaining: "+((int)(dataSet.SessionInfo.SessionTimeRemaining / 60)) + ":" + ((int)dataSet.SessionInfo.SessionTimeRemaining % 60).ToString("00");
            if (dataSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.Laps)
                return "Laps: "+(dataSet.SessionInfo.LeaderCurrentLap + "/" + dataSet.SessionInfo.TotalNumberOfLaps);
            return "NA";
        }

        private void CreateTiming(SimulatorDataSet data)
        {
            var invalidateLap = _shouldReset == ResetModeEnum.Manual ||
                                data.SessionInfo.SessionType != SessionInfo.SessionTypeEnum.Race;
            _timing = SessionTiming.FromSimulatorData(data, invalidateLap);
            _timing.BestLapChangedEvent += BestLapChangedHandler;
            _timing.DriverAdded += Timing_DriverAdded;
            _timing.DriverRemoved += Timing_DriverRemoved;
            
            _gui.Dispatcher.Invoke(() =>
            {
                var mode = _gui.TimingMode;
                _timing.PaceLaps = DisplaySettings.PaceLaps;
                _timing.DisplayBindTimeRelative = (bool)_gui.rbtTimeRelative.IsChecked;
                _timing.DisplayGapToPlayerRelative = !(mode == TimingGui.TimingModeOptions.Absolute ||
                                                      (mode == TimingGui.TimingModeOptions.Automatic &&
                                                       _timing.SessionType != SessionInfo.SessionTypeEnum.Race));
            });
            InitializeGui(data);
            ChangeTimingMode();
            ConnectedSource = data.Source;
            _bestSessionLap = null;
            NotifyPropertyChanged("BestLapFormatted");
            NotifyPropertyChanged("ConnectedSource");
        }

        private void OnSessionStarted(object sender, DataEventArgs args)
        {            
            CreateTiming(args.Data);
        }

        public ICollectionView TimingInfo { get => ViewSource!= null ? ViewSource.View : null; }

        private void InitializeGui(SimulatorDataSet data)
        {

            _gui.Dispatcher.Invoke(() =>
            {
                if (ViewSource == null)
                {                    
                    ViewSource = new CollectionViewSource();
                    ViewSource.Source = Collection;
                    _gui.dtTimig.DataContext = null;
                    _gui.dtTimig.DataContext = this;
                    ChangeTimingMode();
                }
                Collection.Clear();
                foreach (DriverTiming d in _timing.Drivers.Values)
                {
                    Collection.Add(d);
                }

                StringBuilder sb = new StringBuilder();
                sb.Append(data.SessionInfo.TrackName);
                sb.Append(" (");
                sb.Append(data.SessionInfo.TrackLayoutName);

                sb.Append(") - ");
                sb.Append(data.SessionInfo.SessionType);
                _gui.lblTrack.Content = sb.ToString();

                _gui.timingCircle.SetSessionInfo(data);
                _gui.fuelMonitor.ResetFuelMonitor();
            });
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
                PaceLapsChanged();
            if (args?.PropertyName == "RefreshRate")
                _refreshRate = DisplaySettings.RefreshRate;
        }

        private void ApplyDisplaySettings(DisplaySettingsModelView settings)
        {
            if(settings==null)
                return;
            _gui?.Dispatcher.Invoke(() =>
            {
                _gui.whLeftFront.TemperatureDisplayUnit = settings.TemperatureUnits;
                _gui.whRightFront.TemperatureDisplayUnit = settings.TemperatureUnits;
                _gui.whLeftRear.TemperatureDisplayUnit = settings.TemperatureUnits;
                _gui.whRightRear.TemperatureDisplayUnit = settings.TemperatureUnits;

                _gui.whLeftFront.PressureDisplayUnits = settings.PressureUnits;
                _gui.whRightFront.PressureDisplayUnits = settings.PressureUnits;
                _gui.whLeftRear.PressureDisplayUnits = settings.PressureUnits;
                _gui.whRightRear.PressureDisplayUnits = settings.PressureUnits;
            });
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            TimingDataViewModel timingDataViewModel = (TimingDataViewModel) dependencyObject;
            DisplaySettingsModelView newDisplaySettingsModelView =
                (DisplaySettingsModelView) dependencyPropertyChangedEventArgs.NewValue;
            newDisplaySettingsModelView.PropertyChanged += timingDataViewModel.OnDisplaySettingsChange;
            
        }
    }
}
