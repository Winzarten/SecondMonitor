namespace SecondMonitor.DataModel
{
    using Newtonsoft.Json;

    public class DataModelSerializerHelper
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
