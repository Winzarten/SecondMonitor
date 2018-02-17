namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using SecondMonitor.DataModel.BasicProperties;

    public class FuelInfo
    {
        public FuelInfo()
        {
            this.FuelCapacity = new Volume();
            this.FuelRemaining = new Volume();
            this.FuelPressure = new Pressure();
        }

        public Volume FuelCapacity { get; set; }

        public Volume FuelRemaining { get; set; }

        public Pressure FuelPressure { get; set; }
    }
}
