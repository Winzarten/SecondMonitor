namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using System;

    using BasicProperties;

    [Serializable]
    public sealed class OilInfo
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
