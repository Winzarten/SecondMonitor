﻿namespace SecondMonitor.PluginManager.GameConnector
{
    using System;

    using DataModel.Snapshot;

    public class DataEventArgs : EventArgs
    {

        public DataEventArgs(SimulatorDataSet data)
        {
            Data = data;
        }

        public SimulatorDataSet Data
        {
            get;
            set;
        }
    }
}