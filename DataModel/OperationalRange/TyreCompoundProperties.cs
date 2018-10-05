namespace SecondMonitor.DataModel.OperationalRange
{
    using System;

    using SecondMonitor.DataModel.BasicProperties;

    [Serializable]
    public class TyreCompoundProperties
    {
        public TyreCompoundProperties()
        {

        }

        public string CompoundName { get; set; }

        public Pressure IdealPressure { get; set; }

        public Pressure IdealPressureWindow { get; set; }

        public Temperature IdealTemperature { get; set; }

        public Temperature IdealTemperatureWindow { get; set; }
    }
}