namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using SecondMonitor.DataModel.BasicProperties;

    public class WaterInfo
    {
        public WaterInfo()
        {
            this.WaterTemperature = new Temperature();
        }

        public Temperature WaterTemperature { get; set; }
    }
}
