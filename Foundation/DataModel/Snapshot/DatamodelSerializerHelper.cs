namespace SecondMonitor.DataModel.Snapshot
{
    using Newtonsoft.Json;

    public sealed class DataModelSerializerHelper
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
