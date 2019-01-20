using System;

namespace SecondMonitor.DataModel.BasicProperties
{
    using System.Xml.Serialization;

    [Serializable]
    public sealed class TyreWear
    {

        [XmlAttribute]
        public double ActualWear { get; set; }

        [XmlAttribute]
        public double NoWearWearLimit { get; set; }

        [XmlAttribute]
        public double LightWearLimit { get; set; }

        [XmlAttribute]
        public double HeavyWearLimit { get; set; }
    }
}