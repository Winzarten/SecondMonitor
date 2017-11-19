using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecondMonitor.PluginManager.Core;
using SecondMonitor.DataModel;
using SecondMonitor.PluginManager.GameConnector;
using SecondMonitor.Timing.Model;
using SecondMonitor.Timing.GUI;
using SecondMonitor.Timing.Model.Drivers;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using SecondMonitor.Timing.DataHandler.Commands;
using SecondMonitor.Timing.Model.Settings;

namespace SecondMonitor.Timing.DataHandler
{

    public class TimingDataViewModel : ISecondMonitorPlugin, INotifyPropertyChanged
    {
        private int _refreshRate = 1000;
        private enum ResetModeEnum {  NO_RESET, MANUAL, AUTOMATIC}
        private TimingGUI gui;
        private PluginsManager pluginsManager;
        private SessionTiming timing;
        private SessionInfo.SessionTypeEnum sessionType = SessionInfo.SessionTypeEnum.NA;
        private int paceLaps = 3;
        private bool _scrollToPlayer = true;
        ResetModeEnum _shouldReset = ResetModeEnum.NO_RESET;
        public event PropertyChangedEventHandler PropertyChanged;
        private DisplaySettings displaySettings = new DisplaySettings();


        private DateTime lastRefreshTiming;
        private DateTime lastRefreshCarInfo;
        private DateTime lastRefreshCircleInfo;

        // Gets or sets the CollectionViewSource
        public CollectionViewSource ViewSource { get; set; }    

        // Gets or sets the ObservableCollection
        public ObservableCollection<DriverTiming> Collection { get; set; } = new ObservableCollection<DriverTiming>();

        private LapInfo bestSessionLap;
        public string BestLapFormatted { get => bestSessionLap != null ? bestSessionLap.Driver.DriverInfo.DriverName +"-(L"+ bestSessionLap.LapNumber+"):"+ DriverTiming.FormatTimeSpan(bestSessionLap.LapTime) : "Best Session Lap"; }
        public string SessionTime { get => timing != null ? timing.SessionTime.ToString("mm\\:ss\\.fff") : ""; }
        public string ConnectedSource { get; private set; }
        public string SystemTime { get => DateTime.Now.ToString("HH:mm"); }

        public PluginsManager PluginManager
        {
            get => pluginsManager;
            set
            {
                pluginsManager = value;
                pluginsManager.DataLoaded += OnDataLoaded;
                pluginsManager.SessionStarted += OnSessionStarted;
            }
        }


        public bool IsDaemon => false;

        public void RunPlugin()
        {
            ConnectedSource = "Not Connected";
            displaySettings.DisplaySettingsChanged += OnDisplaySettingsChange;
            gui = new TimingGUI();
            gui.Show();
            gui.upDownPaceLaps.Value = paceLaps;
            gui.Closed += Gui_Closed;
            gui.DataContext = this;
            OnDisplaySettingsChange(this, new DisplaySettings.DisplaySettingsChangedArgs(displaySettings));
            lastRefreshTiming = DateTime.Now;
            lastRefreshCarInfo = DateTime.Now;
            _shouldReset = ResetModeEnum.NO_RESET;
        }

        public DisplaySettings DisplaySettings { get => displaySettings; }
        /*public TemperatureUnits TemperatureUnits { get => displaySettings.TemperatureUnits; set => displaySettings.TemperatureUnits = value; }
        public PressureUnits PressureUnits { get => displaySettings.PressureUnits; set => displaySettings.PressureUnits = value; }
        public VolumeUnits VolumeUnits { get => displaySettings.VolumeUnits; set => displaySettings.VolumeUnits = value; }*/

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
                           new TimingModeChangedCommand(ChangeDisplayMode, () => { return timing != null; }));
            }
        }

        public int RefreshRate { get => _refreshRate; set => _refreshRate = value; }
        public int PaceLaps { get => paceLaps;
            set
            {
                paceLaps = value;
                PaceLapsChanged();
            }
        }
        public int SessionCompletedPercentage => timing != null ? timing.SessionCompletedPercentage : 50;

        private NoArgumentCommand _refreshRateCommand;
        public NoArgumentCommand RefreshRateCommand
        {
            get
            {
                return _refreshRateCommand ?? (_refreshRateCommand = new NoArgumentCommand(() =>
                           {
                               _refreshRate = (int) gui.upDownRefreshRate.Value;
                           }));
            }
        }


        private void ScrollToPlayerChanged()
        {
            _scrollToPlayer = (bool)gui.chkScrollToPlayer.IsChecked;
        }

        private void PaceLapsChanged()
        {
            if (timing != null)
                timing.PaceLaps = paceLaps;
            gui.Dispatcher.Invoke(RefreshDatagrid);

        }
        private void ResetFuel()
        {
            gui.fuelMonitor.ResetFuelMonitor();
        }
        private void ScheduleReset()
        {
            _shouldReset = ResetModeEnum.MANUAL;
        }

        private void ChangeTimingMode()
        {
            gui.Dispatcher.Invoke(() =>
            {
                var mode = gui.TimingMode;
                if (mode == TimingGUI.TimingModeOptions.Absolute || (mode == TimingGUI.TimingModeOptions.Automatic && timing.SessionType != SessionInfo.SessionTypeEnum.Race))
                {
                    ViewSource.SortDescriptions.Clear();
                    ViewSource.SortDescriptions.Add(new SortDescription("Position", ListSortDirection.Ascending));
                    timing.DisplayGapToPlayerRelative = false;
                }
                else
                {
                    ViewSource.SortDescriptions.Clear();
                    ViewSource.SortDescriptions.Add(new SortDescription("DistanceToPlayer", ListSortDirection.Ascending));
                    timing.DisplayGapToPlayerRelative = true;
                }
            });
        }

        private void ChangeDisplayMode()
        {
            if (timing == null || gui == null)
                return;
            gui.Dispatcher.Invoke(() =>
            {
                var mode = gui.TimingMode;
                timing.DisplayBindTimeRelative = (bool)gui.rbtTimeRelative.IsChecked;
                timing.DisplayGapToPlayerRelative = !(mode == TimingGUI.TimingModeOptions.Absolute || (mode == TimingGUI.TimingModeOptions.Automatic && timing.SessionType != SessionInfo.SessionTypeEnum.Race));
            });
        }

        private void Gui_Closed(object sender, EventArgs e)
        {
            pluginsManager.DeletePlugin(this);
        }

        private void OnDataLoaded(object sender, DataEventArgs args)
        {
            SimulatorDataSet data = args.Data;
            if (ViewSource == null)
                return;
            //Reset state was detected (either reset button was pressed or timing detected a session change)
            if (_shouldReset != ResetModeEnum.NO_RESET)
            {
                CreateTiming(data);
                _shouldReset = ResetModeEnum.NO_RESET;
            }
            try
            {
                timing?.UpdateTiming(data);
            }catch(SessionTiming.DriverNotFoundException)
            {
                _shouldReset = ResetModeEnum.AUTOMATIC;
                return;
            }
            var timeSpan = DateTime.Now.Subtract(lastRefreshTiming);
           
            if(sessionType!= timing.SessionType)
            {
                _shouldReset = ResetModeEnum.AUTOMATIC;
                sessionType = timing.SessionType;
                return;
            }
            if (timeSpan.TotalMilliseconds > _refreshRate)
            {
                RefreshGui(data);
            }
            TimeSpan timeSpanCarIno = DateTime.Now.Subtract(lastRefreshCarInfo);
            if (timeSpanCarIno.TotalMilliseconds > 33)
            {
                gui.Dispatcher.Invoke((Action)(() =>
                {                    
                    NotifyPropertyChanged("SessionTime");
                    gui.pedalControl.UpdateControl(data);
                    gui.whLeftFront.UpdateControl(data);
                    gui.whRightFront.UpdateControl(data);
                    gui.whLeftRear.UpdateControl(data);
                    gui.whRightRear.UpdateControl(data);
                    gui.fuelMonitor.ProcessDataSet(data);
                }));
                lastRefreshCarInfo = DateTime.Now;
            }
            timeSpanCarIno = DateTime.Now.Subtract(lastRefreshCircleInfo);
            if (timeSpanCarIno.TotalMilliseconds > 200)
            {
                gui.Dispatcher.Invoke((Action)(() =>
                {
                    gui.timingCircle.RefreshSession(data);
                }));
                lastRefreshCircleInfo = DateTime.Now;
            }
        }

        

        private void SynchronizeTimingWithCollection(SimulatorDataSet data)
        {
            
        }

        private void Timing_DriverRemoved(object sender, DriverListModificationEventArgs e)
        {
            gui?.Dispatcher.Invoke(() =>
            {
                gui.timingCircle.RemoveDriver(e.Data.DriverInfo);
                Collection?.Remove(e.Data);
            });

        }

        private void Timing_DriverAdded(object sender, DriverListModificationEventArgs e)
        {
            gui?.Dispatcher.Invoke(() =>
            {
                Collection?.Add(e.Data);
                gui.timingCircle.AddDriver(e.Data.DriverInfo);
            });
            

        }

        private void RefreshGui(SimulatorDataSet data)
        {
            gui.Dispatcher.Invoke((Action)(() =>
            {
                NotifyPropertyChanged("SystemTime");
                NotifyPropertyChanged("SessionCompletedPercentage");
                gui.pedalControl.UpdateControl(data);
                gui.whLeftFront.UpdateControl(data);
                gui.whRightFront.UpdateControl(data);
                gui.whLeftRear.UpdateControl(data);
                gui.whRightRear.UpdateControl(data);
                gui.waterTemp.Temperature = data.PlayerInfo.CarInfo.WaterSystmeInfo.WaterTemperature;
                gui.oilTemp.Temperature = data.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature;
                gui.timingCircle.RefreshSession(data);


                gui.lblWeather.Content = "Air: " + data.SessionInfo.WeatherInfo.airTemperature.InCelsius.ToString("n1") + " |Track: " + data.SessionInfo.WeatherInfo.trackTemperature.InCelsius.ToString("n1")
                + "| Rain Intensity: " + data.SessionInfo.WeatherInfo.rainIntensity + "%";
                gui.lblRemainig.Content = GetSessionRemainig(data);
                RefreshDatagrid();
                if (_scrollToPlayer && gui != null && timing.Player != null && gui.dtTimig.Items.Count > 0)
                {
                    gui.dtTimig.ScrollIntoView(gui.dtTimig.Items[0]);
                    gui.dtTimig.ScrollIntoView(timing.Player);
                }
            }));
            lastRefreshTiming = DateTime.Now;
        }

        private void RefreshDatagrid()
        {
            if (ViewSource != null)
                ViewSource.View.Refresh();
        }

        private string GetSessionRemainig(SimulatorDataSet dataSet)
        {
            if (dataSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.NA)
                return "NA";
            if (dataSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.Time)
                return "Time Remaining: "+((int)(dataSet.SessionInfo.SessionTimeRemaining / 60)) + ":" + ((int)dataSet.SessionInfo.SessionTimeRemaining % 60);
            if (dataSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.Laps)
                return "Laps: "+(dataSet.SessionInfo.LeaderCurrentLap + "/" + dataSet.SessionInfo.TotalNumberOfLaps);
            return "NA";
        }

        private void CreateTiming(SimulatorDataSet data)
        {
            var invalidateLap = _shouldReset == ResetModeEnum.MANUAL ||
                                data.SessionInfo.SessionType != SessionInfo.SessionTypeEnum.Race;
            timing = SessionTiming.FromSimulatorData(data, invalidateLap);
            timing.BestLapChangedEvent += BestLapChangedHandler;
            timing.DriverAdded += Timing_DriverAdded;
            timing.DriverRemoved += Timing_DriverRemoved;
            timing.PaceLaps = paceLaps;

            gui.Dispatcher.Invoke(() =>
            {
                var mode = gui.TimingMode;
                timing.DisplayBindTimeRelative = (bool)gui.rbtTimeRelative.IsChecked;
                timing.DisplayGapToPlayerRelative = !(mode == TimingGUI.TimingModeOptions.Absolute ||
                                                      (mode == TimingGUI.TimingModeOptions.Automatic &&
                                                       timing.SessionType != SessionInfo.SessionTypeEnum.Race));
            });            
            InitializeGui(data);
            ChangeTimingMode();
            ConnectedSource = data.Source;
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

            gui.Dispatcher.Invoke(() =>
            {
                if (ViewSource == null)
                {                    
                    ViewSource = new CollectionViewSource();
                    ViewSource.Source = Collection;
                    gui.dtTimig.DataContext = null;
                    gui.dtTimig.DataContext = this;
                    ChangeTimingMode();
                }
                Collection.Clear();
                foreach (DriverTiming d in timing.Drivers.Values)
                {
                    Collection.Add(d);
                }

                StringBuilder sb = new StringBuilder();
                sb.Append(data.SessionInfo.TrackName);
                sb.Append(" (");
                sb.Append(data.SessionInfo.TrackLayoutName);

                sb.Append(") - ");
                sb.Append(data.SessionInfo.SessionType);
                gui.lblTrack.Content = sb.ToString();

                gui.timingCircle.SetSessionInfo(data);
                gui.fuelMonitor.ResetFuelMonitor();
            });
            NotifyPropertyChanged("BestLapFormatted");
        }

        private void BestLapChangedHandler(object sender, SessionTiming.BestLapChangedArgs args)
        {
            bestSessionLap = args.Lap;
            NotifyPropertyChanged("BestLapFormatted");
        }

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnDisplaySettingsChange(object sender, DisplaySettings.DisplaySettingsChangedArgs args)
        {
            ApplyDisplaySettings(args.Settings);
        }

        private void ApplyDisplaySettings(DisplaySettings settings)
        {
            if (gui == null)
                return;
            gui.Dispatcher.Invoke(() =>
                {
                    gui.whLeftFront.TemperatureDisplayUnit = settings.TemperatureUnits;
                    gui.whRightFront.TemperatureDisplayUnit = settings.TemperatureUnits;
                    gui.whLeftRear.TemperatureDisplayUnit = settings.TemperatureUnits;
                    gui.whRightRear.TemperatureDisplayUnit = settings.TemperatureUnits;

                    gui.whLeftFront.PressureDisplayUnits = settings.PressureUnits;
                    gui.whRightFront.PressureDisplayUnits = settings.PressureUnits;
                    gui.whLeftRear.PressureDisplayUnits = settings.PressureUnits;
                    gui.whRightRear.PressureDisplayUnits = settings.PressureUnits;

                    gui.oilTemp.DisplayUnit = settings.TemperatureUnits;
                    gui.waterTemp.DisplayUnit = settings.TemperatureUnits;

                    gui.fuelMonitor.DisplayUnits = settings.VolumeUnits;


                });
        }
    }
}
