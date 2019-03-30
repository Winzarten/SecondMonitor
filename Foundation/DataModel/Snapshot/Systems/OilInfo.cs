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
            OptimalOilTemperature = new OptimalQuantity<Temperature>()
            {
                ActualQuantity = Temperature.Zero,
                IdealQuantity = Temperature.FromCelsius(100),
                IdealQuantityWindow = Temperature.FromCelsius(15),
            };
        }

        public OptimalQuantity<Temperature> OptimalOilTemperature { get; set; }

        public Pressure OilPressure { get; set; }
    }
}
