namespace SecondMonitor.Timing.Model
{
    using System;

    using SecondMonitor.Timing.Model.Drivers.ModelView;

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