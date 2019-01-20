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
        }

        public Temperature WaterTemperature { get; set; }
    }
}
