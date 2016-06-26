using SecondMonitor.DataModel.BasicProperties;

namespace SecondMonitor.DataModel
{
    public class FuelInfo
    {
        public FuelInfo()
        {
            FuelCapacity = new Volume();
            FuelRemaining = new Volume();
            FuelPressure = new Pressure();
        }
        public Volume FuelCapacity;
        public Volume FuelRemaining;
        public Pressure FuelPressure;
    }
}
