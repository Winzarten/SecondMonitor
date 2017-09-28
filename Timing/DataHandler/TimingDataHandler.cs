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

namespace SecondMonitor.Timing.DataHandler
{
    public class ResetCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action executeDelegate;
        private Func<bool> canExecuteDelegate;

        public ResetCommand(Action execute)
        {
            executeDelegate = execute;
            canExecuteDelegate = () => { return true; };
        }
        public ResetCommand(Action execute, Func<bool> canExecute)
        {
            executeDelegate = execute;
            canExecuteDelegate = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteDelegate();
        }

        public void Execute(object parameter)
        {
            executeDelegate();
        }
    }
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
            lastRefreshTiming = DateTime.Now;
            lastRefreshCarInfo = DateTime.Now;
            shouldReset = false;
        }

        private ICommand _resetCommand;
        public ICommand ResetCommand
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

        private void ScheduleReset()
        {
            shouldReset = true;
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
            if (timing != null)
                timing.UpdateTiming(data);
            TimeSpan timeSpan = DateTime.Now.Subtract(lastRefreshTiming);
           
            
            if (timeSpan.TotalMilliseconds >700)
            {
                lastRefreshTiming = DateTime.Now;
                gui.Dispatcher.Invoke((Action)(() =>
                {                                        
                    gui.lblTime.Content = data.SessionInfo.SessionTime.ToString("mm\\:ss\\.fff");
                    gui.pedalControl.UpdateControl(data);
                    gui.whLeftFront.UpdateControl(data);
                    gui.whRightFront.UpdateControl(data);
                    gui.whLeftRear.UpdateControl(data);
                    gui.whRightRear.UpdateControl(data);
                    //gui.gMeter.VertG = -data.PlayerCarInfo.Acceleration.ZInG;
                    //gui.gMeter.HorizG = data.PlayerCarInfo.Acceleration.XInG;
                    //gui.gMeter.Refresh();
                    ViewSource.View.Refresh();
                }));
            }
            TimeSpan timeSpanCarIno = DateTime.Now.Subtract(lastRefreshCarInfo);
            if (timeSpanCarIno.TotalMilliseconds > 33)
            {
                gui.Dispatcher.Invoke((Action)(() =>
                {
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
                    ViewSource.SortDescriptions.Add(new SortDescription("Position", ListSortDirection.Ascending));                    
                    //ViewSource.SortDescriptions.Add(new SortDescription("DistanceToPlayer", ListSortDirection.Ascending));
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
