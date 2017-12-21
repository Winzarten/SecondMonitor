namespace SecondMonitor.Timing.SessionTiming.Drivers
{
    using System;

    using SecondMonitor.Timing.SessionTiming.Drivers.Presentation.ViewModel;

    public class DriverListModificationEventArgs : EventArgs
    {

        public DriverListModificationEventArgs(DriverTimingModelView data)
        {
            Data = data;
        }

        public DriverTimingModelView Data
        {
            get;
            set;
        }
    }
}