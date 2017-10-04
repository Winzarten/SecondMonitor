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
using System.Windows.Input;
using SecondMonitor.Timing.DataHandler.Commands;

namespace SecondMonitor.Timing.DataHandler
{

    public class TimingDataHandler : ISecondMonitorPlugin, INotifyPropertyChanged
    {
        private TimingGUI gui;
        private PluginsManager pluginsManager;
        private SessionTiming timing;
        bool shouldReset;
        public event PropertyChangedEventHandler PropertyChanged;


        private DateTime lastRefreshTiming;
        private DateTime lastRefreshCarInfo;

        // Gets or sets the CollectionViewSource
        public CollectionViewSource ViewSource { get; set; }    

        // Gets or sets the ObservableCollection
        public ObservableCollection<Driver> Collection { get; set; }

        private LapInfo bestSessionLap;
        public string BestLapFormatted { get => bestSessionLap != null ? bestSessionLap.Driver.DriverInfo.DriverName +"-"+ Driver.FormatTimeSpan(bestSessionLap.LapTime) : "Best Session Lap"; }

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
            gui = new TimingGUI();
            gui.Show();
            gui.Closed += Gui_Closed;
            gui.btnReset.DataContext = this;
            gui.rbtAbsolute.DataContext = this;
            gui.rbtAutomatic.DataContext = this;
            gui.rbtRelative.DataContext = this;
            lastRefreshTiming = DateTime.Now;
            lastRefreshCarInfo = DateTime.Now;
            shouldReset = false;
        }

        private ResetCommand _resetCommand;
        public ResetCommand ResetCommand
        {
            get
            {
                if(_resetCommand==null)
                {
                    _resetCommand = new ResetCommand(ScheduleReset);
                }
                return _resetCommand;
            }
        }

        private TimingModeChangedCommand _timingModeChangedCommand;
        public TimingModeChangedCommand TimingModeChangedCommand
        {
            get
            {
                if(_timingModeChangedCommand == null)
                {
                    _timingModeChangedCommand = new TimingModeChangedCommand(ChangeTimingMode, () => {
                        return ViewSource != null;
                    });
                }
                return _timingModeChangedCommand;
            }
        }


        private void ScheduleReset()
        {
            shouldReset = true;
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
                }
                else
                {
                    ViewSource.SortDescriptions.Clear();
                    ViewSource.SortDescriptions.Add(new SortDescription("DistanceToPlayer", ListSortDirection.Ascending));
                }
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
            if(shouldReset)
            {
                timing = SessionTiming.FromSimulatorData(args.Data);
                timing.BestLapChangedEvent += BestLapChangedHandler;
                InitializeGui(data);
                shouldReset = false;
            }
            try
            {
                if (timing != null)
                    timing.UpdateTiming(data);
            }catch(SessionTiming.DriverNotFoundException)
            {
                shouldReset = true;
                return;
            }
            TimeSpan timeSpan = DateTime.Now.Subtract(lastRefreshTiming);
           
            
            if (timeSpan.TotalMilliseconds >500)
            {
                lastRefreshTiming = DateTime.Now;
                gui.Dispatcher.Invoke((Action)(() =>
                {
                    gui.lblTime.Content = "Session Time: " + data.SessionInfo.SessionTime.ToString("mm\\:ss\\.fff");
                    gui.pedalControl.UpdateControl(data);
                    gui.whLeftFront.UpdateControl(data);
                    gui.whRightFront.UpdateControl(data);
                    gui.whLeftRear.UpdateControl(data);
                    gui.whRightRear.UpdateControl(data);
                    //gui.gMeter.VertG = -data.PlayerCarInfo.Acceleration.ZInG;
                    //gui.gMeter.HorizG = data.PlayerCarInfo.Acceleration.XInG;
                    //gui.gMeter.Refresh();
                    gui.timingCircle.RefreshSession(data);
                    ViewSource.View.Refresh();
                }));
            }
            TimeSpan timeSpanCarIno = DateTime.Now.Subtract(lastRefreshCarInfo);
            if (timeSpanCarIno.TotalMilliseconds > 33)
            {
                gui.Dispatcher.Invoke((Action)(() =>
                {
                    gui.lblTime.Content = "Session Time: " + data.SessionInfo.SessionTime.ToString("mm\\:ss\\.fff");
                    gui.pedalControl.UpdateControl(data);
                    gui.whLeftFront.UpdateControl(data);
                    gui.whRightFront.UpdateControl(data);
                    gui.whLeftRear.UpdateControl(data);
                    gui.whRightRear.UpdateControl(data);                    
                    //gui.gMeter.VertG = -data.PlayerCarInfo.Acceleration.ZInG;
                    //gui.gMeter.HorizG = data.PlayerCarInfo.Acceleration.XInG;
                    //gui.gMeter.Refresh();
                }));
                lastRefreshCarInfo = DateTime.Now;
            }
        }

        private void OnSessionStarted(object sender, DataEventArgs args)
        {            
            timing = SessionTiming.FromSimulatorData(args.Data);
            timing.BestLapChangedEvent += BestLapChangedHandler;
            InitializeGui(args.Data);
            ChangeTimingMode();            
        }

        public ICollectionView TimingInfo { get => ViewSource.View; }

        private void InitializeGui(SimulatorDataSet data)
        {

            gui.Dispatcher.Invoke(() =>
            {
                if (Collection == null)
                {
                    Collection = new ObservableCollection<Driver>();
                    ViewSource = new CollectionViewSource();
                    ViewSource.Source = Collection;
                    ChangeTimingMode();                    
                    
                    gui.dtTimig.DataContext = this;
                    gui.lblBestLap.DataContext = this;
                }
                Collection.Clear();
                foreach (Driver d in timing.Drivers.Values)
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
                
            });
            NotifyPropertyChanged();
        }

        private void BestLapChangedHandler(object sender, SessionTiming.BestLapChangedArgs args)
        {
            bestSessionLap = args.Lap;
            NotifyPropertyChanged();
        }

        protected virtual void NotifyPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BestLapFormatted"));
        }
    }
}
