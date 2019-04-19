namespace SecondMonitor.Timing.SessionTiming.Drivers
{
    using System;

    using ViewModel;

    public class DriverListModificationEventArgs : EventArgs
    {

        public DriverListModificationEventArgs(DriverTiming data)
        {
            Data = data;
        }

        public DriverTiming Data
        {
            get;
            set;
        }
    }
}