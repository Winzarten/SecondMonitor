namespace SecondMonitor.Rating.Common.DataModel.Player
{
    using System.Xml.Serialization;

    public class DriversRating
    {
        [XmlAttribute]
        public double Rating { get; set; }

        [XmlAttribute]
        public double Deviation { get; set; }

        [XmlAttribute]
        public double Volatility { get; set; }
    }
}