namespace SecondMonitor.Timing.SessionTiming
{
    using System;
    using Drivers.ViewModel;

    public class LapEventArgs : EventArgs
    {
        public LapEventArgs(LapInfo lapInfo)
        {
            Lap = lapInfo;
        }

        public LapInfo Lap
        {
            get;
        }
    }
}