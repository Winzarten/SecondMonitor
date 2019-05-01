namespace SecondMonitor.Rating.Common.DataModel.Player
{
    using System;
    using System.Xml.Serialization;

    public struct DriversRating
    {
        private DateTime _creationTime;

        [XmlAttribute]
        public int Rating { get; set; }

        [XmlAttribute]
        public int Deviation { get; set; }

        [XmlAttribute]
        public double Volatility { get; set; }

        [XmlIgnore]
        public DateTime CreationTime
        {
            get => _creationTime;
            set => _creationTime = value;
        }

        [XmlAttribute]
        public string CreationTimeFormatted
        {
            get => _creationTime.ToString("yyyy-MM-dd HH:mm:ss");
            set => _creationTime = DateTime.Parse(value);
        }
    }
}