namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using SecondMonitor.DataModel.BasicProperties;

    public class WaterInfo
    {
        public WaterInfo()
        {
            WaterTemperature = Temperature.Zero;
        }

        public Temperature WaterTemperature { get; set; }
    }
}
