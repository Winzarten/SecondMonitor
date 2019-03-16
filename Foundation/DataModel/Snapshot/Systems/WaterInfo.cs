namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using System;

    using BasicProperties;

    [Serializable]
    public sealed class WaterInfo
    {
        public WaterInfo()
        {
            WaterTemperature = Temperature.Zero;
            WaterPressure = Pressure.Zero;
        }

        public Temperature WaterTemperature { get; set; }

        public Pressure WaterPressure { get; set; }
    }
}
