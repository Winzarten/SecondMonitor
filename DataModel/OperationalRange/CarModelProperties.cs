namespace SecondMonitor.DataModel.OperationalRange
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    using BasicProperties;

    [Serializable]
    public class CarModelProperties
    {
        public CarModelProperties()
        {
            TyreCompoundsProperties = new List<TyreCompoundProperties>();
            WheelRotation = 540;
        }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public int WheelRotation { get; set; }

        public Temperature OptimalBrakeTemperature { get; set; }
        public Temperature OptimalBrakeTemperatureWindow { get; set; }
        public List<TyreCompoundProperties> TyreCompoundsProperties { get; set; }

        public TyreCompoundProperties GetTyreCompound(string compoundName)
        {
            return TyreCompoundsProperties.FirstOrDefault(x => x.CompoundName == compoundName);
        }

        public void AddTyreCompound(TyreCompoundProperties newCompound)
        {
            TyreCompoundsProperties.Add(newCompound);
        }
    }
}