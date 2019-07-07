namespace SecondMonitor.Rating.Common.DataModel.Player
{
    using System.Xml.Serialization;

    public class DriverWithoutRating
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public int Deviation { get; set; }

        [XmlAttribute]
        public double Volatility { get; set; }
    }
}