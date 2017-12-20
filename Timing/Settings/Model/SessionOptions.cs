using System;

namespace SecondMonitor.Timing.Model.Settings.Model
{
    using SecondMonitor.Timing.Settings.Model;

    [Serializable]
    public class SessionOptions
    {
        public string SessionName { get; set; } = "NA";

        public DisplayModeEnum TimesDisplayMode { get; set; } = DisplayModeEnum.Relative;

        public DisplayModeEnum OrderingDisplayMode { get; set; } = DisplayModeEnum.Absolute;

        public ColumnsSettings ColumnsSettings { get; set; } = new ColumnsSettings();
    }
}
