namespace SecondMonitor.DataModel.OperationalRange
{
    using System;
    using System.Collections.Generic;

    using SecondMonitor.DataModel.BasicProperties;

    [Serializable]
    public class CarModelProperties
    {
        public CarModelProperties()
        {
            CompoundProperties = new Dictionary<string, TyreCompoundProperties>();
        }

        public string Name { get; set; }

        public Temperature OptimalBrakeTemperature { get; set; }
        public Temperature OptimalBrakeTemperatureWindow { get; set; }
        public Dictionary<string, TyreCompoundProperties> CompoundProperties { get; set; }
    }
}