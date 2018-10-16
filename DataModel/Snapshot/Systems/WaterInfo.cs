namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using System;

    using BasicProperties;

    [Serializable]
    public class WaterInfo
    {
        public WaterInfo()
        {
            WaterTemperature = Temperature.Zero;
        }

        public Temperature WaterTemperature { get; set; }
    }
}
