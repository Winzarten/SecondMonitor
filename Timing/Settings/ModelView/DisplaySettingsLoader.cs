namespace SecondMonitor.Timing.Settings.ModelView
{
    using System;
    using System.IO;

    using Newtonsoft.Json;

    using NLog;

    using Model;

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
                LogManager.GetCurrentClassLogger().Error(ex, "Error while loading display settings - default settings created");
                return new DisplaySettings();
            }
            
        }
    }
}
