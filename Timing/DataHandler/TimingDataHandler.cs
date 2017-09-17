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

namespace SecondMonitor.Timing.DataHandler
{
    public class TimingDataHandler : ISecondMonitorPlugin
    {
        private TimingGUI gui;
        private PluginsManager pluginsManager;
        private SessionTiming timing;

        private DateTime lastRefresh;

        // Gets or sets the CollectionViewSource
        public CollectionViewSource ViewSource { get; set; }

        // Gets or sets the ObservableCollection
        public ObservableCollection<Driver> Collection { get; set; }

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
            lastRefresh = DateTime.Now;
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
            if (timing != null)
                timing.UpdateTiming(data);
            TimeSpan timeSpan = DateTime.Now.Subtract(lastRefresh);
            if (timeSpan.TotalMilliseconds > 1000)
            {
                lastRefresh = DateTime.Now;
                gui.Dispatcher.Invoke(() =>
                {
                    ViewSource.View.Refresh();                    
                });
            }
        }

        private void OnSessionStarted(object sender, DataEventArgs args)
        {
            timing = SessionTiming.FromSimulatorData(args.Data);
            InitializeGui(args.Data);
        }

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
                    gui.dtTimig.ItemsSource = ViewSource.View;                    
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
        }
    }
}
