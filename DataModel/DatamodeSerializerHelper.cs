using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SecondMonitor.DataModel
{
    public class DatamodeSerializerHelper
    {
        public static string ToJson(SimulatorDataSet value)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(value, Formatting.Indented, settings);
        }
    }
}
