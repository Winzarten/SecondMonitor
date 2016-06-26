using SecondMonitor.Core.R3EConnector.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecondMonitor.DataModel;

namespace SecondMonitor.Core.R3EConnector
{
    public class DataEventArgs : EventArgs
    {
        
        public DataEventArgs(SimulatorDataSet data)
        {
            this.Data = data;
        }

        public SimulatorDataSet Data
        {
            get;
            set;
        }
    }

    public interface IR3EConnector
    {
        event EventHandler<DataEventArgs> DataLoaded;
        event EventHandler<EventArgs> ConnectedEvent;
        event EventHandler<EventArgs> Disconnected;

        bool IsConnected
        {
            get;            
        }
        int TickTime
        {
            get;
            set;
        }
        bool TryConnect();
        void AsynConnect();

    }
}
