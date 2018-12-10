namespace SecondMonitor.ViewModels.Settings.Model
{
    using System;

    [Serializable]
    public class SessionReportSettings
    {
        public bool Export { get; set; }
        public bool AutoOpen { get; set; }
    }
}