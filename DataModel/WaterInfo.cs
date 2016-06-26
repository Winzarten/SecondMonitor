namespace SecondMonitor.DataModel
{
    public class WaterInfo
    {
        public WaterInfo()
        {
            WaterTemperature = new Temperature();
        }
        public Temperature WaterTemperature;
    }
}
