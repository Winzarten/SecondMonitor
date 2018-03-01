namespace SecondMonitor.Timing.Settings.Model
{
    using System;

    [Serializable]
    public class SessionReportSettings
    {
        public bool Export { get; set; }
        public bool AutoOpen { get; set; }
    }
}