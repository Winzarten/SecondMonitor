using System;
using System.IO;

using Newtonsoft.Json;
using NLog;
using SecondMonitor.Timing.Model.Settings.Model;

namespace SecondMonitor.Timing.Model.Settings.ModelView
{
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
