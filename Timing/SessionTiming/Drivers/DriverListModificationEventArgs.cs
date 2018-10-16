namespace SecondMonitor.Timing.SessionTiming.Drivers
{
    using System;

    using SecondMonitor.Timing.SessionTiming.Drivers.Presentation.ViewModel;

    public class DriverListModificationEventArgs : EventArgs
    {

        public DriverListModificationEventArgs(DriverTimingViewModel data)
        {
            Data = data;
        }

        public DriverTimingViewModel Data
        {
            get;
            set;
        }
    }
}