namespace SecondMonitor.DataModel
{
    public class OilInfo
    {
        public OilInfo()
        {
            OilPressure = new Pressure();
            OilTemperature = new Temperature();
        }
        public Temperature OilTemperature;
        public Pressure OilPressure;
    }
}
