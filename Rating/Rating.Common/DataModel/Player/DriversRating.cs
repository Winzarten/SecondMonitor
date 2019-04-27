namespace SecondMonitor.Rating.Common.DataModel.Player
{
    using System.Xml.Serialization;

    public class DriversRating
    {
        [XmlAttribute]
        public int Rating { get; set; }

        [XmlAttribute]
        public int Deviation { get; set; }

        [XmlAttribute]
        public double Volatility { get; set; }
    }
}