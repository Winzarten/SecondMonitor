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
