using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                return JsonConvert.DeserializeObject<DisplaySettings>(File.ReadAllText(fileName));
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Error while loading display settings - default settings created");
                return new DisplaySettings();;
            }
            
        }
    }
}
