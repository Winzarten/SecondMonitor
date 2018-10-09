namespace SecondMonitor.Timing.Settings.ViewModel
{
    using System;
    using System.IO;

    using Newtonsoft.Json;

    using NLog;

    using SecondMonitor.Timing.Settings.Model;

    public class DisplaySettingsLoader
    {
        public DisplaySettings LoadDisplaySettingsFromFileSafe(string fileName)
        {
            try
            {
                object deserializedOptions = JsonConvert.DeserializeObject<DisplaySettings>(File.ReadAllText(fileName));
                if (deserializedOptions == null)
                {
                    return new DisplaySettings();
                }

                return (DisplaySettings)deserializedOptions;
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Error while loading display settingsView - default settingsView created");
                return new DisplaySettings();
            }
        }
    }
}
