namespace SecondMonitor.ViewModels.Settings.ViewModel
{
    using System;
    using System.IO;
    using Model;
    using Newtonsoft.Json;
    using NLog;

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

        public bool TrySaveDisplaySettings(DisplaySettings displaySettings, string filePath)
        {
            try
            {
                File.WriteAllText(filePath, JsonConvert.SerializeObject(displaySettings, Formatting.Indented));
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Error while saving display settingsView.");
                return false;
            }

            return true;
        }
    }
}
