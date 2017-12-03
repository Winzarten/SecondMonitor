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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SecondMonitor.Timing.DataHandler.Commands;
using SecondMonitor.Timing.Model.Drivers.Visualizer;
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
        private SimulatorDataSet _lastDataSet;
      
        // Gets or sets the CollectionViewSource
        public CollectionViewSource ViewSource { get; set; } 

        // Gets or sets the ObservableCollection
        public ObservableCollection<DriverTimingVisualizer> Collection { get; set; } = new ObservableCollection<DriverTimingVisualizer>();

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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            //This is expected as these are refresh tasks that should exist during the instance life
            SchedulePeriodicAction(new Action(() => RefreshGui(_lastDataSet)), 10000, this, CancellationToken.None);
            SchedulePeriodicAction(new Action(() => RefreshBasicInfo(_lastDataSet)), 33, this, CancellationToken.None);
            SchedulePeriodicAction(new Action(() => RefreshTimingCircle(_lastDataSet)), 300, this, CancellationToken.None);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            OnDisplaySettingsChange(this, null);
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
            _scrollToPlayer = (bool)_gui.ChkScrollToPlayer.IsChecked;
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
                    ViewSource.SortDescriptions.Add(new SortDescription("DriverTiming.Position", ListSortDirection.Ascending));
                    _timing.DisplayGapToPlayerRelative = false;
                }
                else
                {
                    ViewSource.SortDescriptions.Clear();
                    ViewSource.SortDescriptions.Add(new SortDescription("DriverTiming.DistanceToPlayer", ListSortDirection.Ascending));
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
                _timing.DisplayBindTimeRelative = (bool)_gui.RbtTimeRelative.IsChecked;
                _timing.DisplayGapToPlayerRelative = !(mode == TimingGui.TimingModeOptions.Absolute || (mode == TimingGui.TimingModeOptions.Automatic && _timing.SessionType != SessionInfo.SessionTypeEnum.Race));
            });
        }

        private void Gui_Closed(object sender, EventArgs e)
        {
            _pluginsManager.DeletePlugin(this);
        }

        private void OnDataLoaded(object sender, DataEventArgs args)
        {
            _lastDataSet = args.Data;
            if (this.Dispatcher.CheckAccess())
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
                    return;
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
                return;
            _gui.TimingCircle.RefreshSession(data);
        }

        private void RefreshBasicInfo(SimulatorDataSet data)
        {
            if (data == null)
                return;
            NotifyPropertyChanged("SessionTime");
            NotifyPropertyChanged("SystemTime");
            NotifyPropertyChanged("SessionCompletedPercentage");
            _gui.PedalControl.UpdateControl(data);
            _gui.WhLeftFront.UpdateControl(data);
            _gui.WhRightFront.UpdateControl(data);
            _gui.WhLeftRear.UpdateControl(data);
            _gui.WhRightRear.UpdateControl(data);
            _gui.FuelMonitor.ProcessDataSet(data);
            _gui.WaterTemp.Temperature = data.PlayerInfo.CarInfo.WaterSystmeInfo.WaterTemperature;
            _gui.OilTemp.Temperature = data.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature;

            _gui.LblWeather.Content = "Air: " + data.SessionInfo.WeatherInfo.AirTemperature.InCelsius.ToString("n1") + " |Track: " + data.SessionInfo.WeatherInfo.TrackTemperature.InCelsius.ToString("n1")
                                      + "| Rain Intensity: " + data.SessionInfo.WeatherInfo.RainIntensity + "%";
            _gui.LblRemainig.Content = GetSessionRemainig(data);
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
            if(data == null)
                return;
            if(this.Dispatcher.CheckAccess())
            {
                _gui.PedalControl.UpdateControl(data);
                _gui.WhLeftFront.UpdateControl(data);
                _gui.WhRightFront.UpdateControl(data);
                _gui.WhLeftRear.UpdateControl(data);
                _gui.WhRightRear.UpdateControl(data);
                _gui.TimingCircle.RefreshSession(data);
                RefreshDatagrid();
                if (_scrollToPlayer && _gui != null && _timing.Player != null && _gui.DtTimig.Items.Count > 0)
                {
                    _gui.DtTimig.ScrollIntoView(_gui.DtTimig.Items[0]);
                    _gui.DtTimig.ScrollIntoView(_timing.Player);
                }
            }
            else
            {
                this.Dispatcher.Invoke(()=> RefreshGui(data));
            }
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
            
            
            this.Dispatcher.Invoke(() =>
            {
                _timing = SessionTiming.FromSimulatorData(data, invalidateLap, this);
                _timing.BestLapChangedEvent += BestLapChangedHandler;
                _timing.DriverAdded += Timing_DriverAdded;
                _timing.DriverRemoved += Timing_DriverRemoved;
                var mode = _gui.TimingMode;
                _timing.PaceLaps = DisplaySettings.PaceLaps;
                _timing.DisplayBindTimeRelative = (bool)_gui.RbtTimeRelative.IsChecked;
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
                    _gui.DtTimig.DataContext = null;
                    _gui.DtTimig.DataContext = this;
                    ChangeTimingMode();
                }
                Collection.Clear();
                foreach (DriverTimingVisualizer d in _timing.Drivers.Values)
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
                _gui.WhLeftFront.TemperatureDisplayUnit = settings.TemperatureUnits;
                _gui.WhRightFront.TemperatureDisplayUnit = settings.TemperatureUnits;
                _gui.WhLeftRear.TemperatureDisplayUnit = settings.TemperatureUnits;
                _gui.WhRightRear.TemperatureDisplayUnit = settings.TemperatureUnits;

                _gui.WhLeftFront.PressureDisplayUnits = settings.PressureUnits;
                _gui.WhRightFront.PressureDisplayUnits = settings.PressureUnits;
                _gui.WhLeftRear.PressureDisplayUnits = settings.PressureUnits;
                _gui.WhRightRear.PressureDisplayUnits = settings.PressureUnits;
            });
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            TimingDataViewModel timingDataViewModel = (TimingDataViewModel) dependencyObject;
            DisplaySettingsModelView newDisplaySettingsModelView =
                (DisplaySettingsModelView) dependencyPropertyChangedEventArgs.NewValue;
            newDisplaySettingsModelView.PropertyChanged += timingDataViewModel.OnDisplaySettingsChange;
            
        }

        private static async Task SchedulePeriodicAction(Action action, int periodInMS, object sender, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(periodInMS, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                    action();
            }
        }
    }
}
