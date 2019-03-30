namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using System;

    using BasicProperties;

    [Serializable]
    public sealed class WaterInfo
    {
        public WaterInfo()
        {
            //WaterTemperature = Temperature.Zero;
            OptimalWaterTemperature = new OptimalQuantity<Temperature>()
            {
                ActualQuantity = Temperature.Zero,
                IdealQuantity = Temperature.FromCelsius(90),
                IdealQuantityWindow = Temperature.FromCelsius(10),
            };

            WaterPressure = Pressure.Zero;
        }


        //public Temperature WaterTemperature { get; set; }

        public OptimalQuantity<Temperature> OptimalWaterTemperature { get; set; }

        public Pressure WaterPressure { get; set; }
    }
}
