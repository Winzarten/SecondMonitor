namespace SecondMonitor.DataModel.OperationalRange
{
    using System;
    using System.Xml.Serialization;

    using BasicProperties;

    [Serializable]
    public sealed class TyreCompoundProperties
    {
        public TyreCompoundProperties()
        {
            NoWearLimit = 0.1;
            LowWearLimit = 0.25;
            HeavyWearLimit = 0.7;
        }

        [XmlAttribute]
        public string CompoundName { get; set; }

        [XmlAttribute]
        public double NoWearLimit { get; set; }

        [XmlAttribute]
        public double LowWearLimit { get; set; }

        [XmlAttribute]
        public double HeavyWearLimit { get; set; }

        public Pressure IdealPressure { get; set; }

        public Pressure IdealPressureWindow { get; set; }

        public Temperature IdealTemperature { get; set; }

        public Temperature IdealTemperatureWindow { get; set; }

    }
}