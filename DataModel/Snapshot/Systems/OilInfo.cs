namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using SecondMonitor.DataModel.BasicProperties;

    public class OilInfo
    {
        public OilInfo()
        {
            OilPressure = new Pressure();
            OilTemperature = Temperature.Zero;
        }

        public Temperature OilTemperature { get; set; }

        public Pressure OilPressure { get; set; }
    }
}
