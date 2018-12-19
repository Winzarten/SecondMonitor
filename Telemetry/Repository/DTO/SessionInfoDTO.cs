namespace SecondMonitor.Telemetry.TelemetryManagement.DTO
{
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "SessionInfo")]
    public class SessionInfoDto
    {

        [XmlAttribute]
        public string Simulator { get; set; }

        [XmlAttribute]
        public string TrackName { get; set; }

        [XmlAttribute]
        public string LayoutName { get; set; }

        [XmlAttribute]
        public double LayoutLength { get; set; }

        [XmlAttribute]
        public string PlayerName { get; set; }

        [XmlAttribute]
        public string CarName { get; set; }
    }
}