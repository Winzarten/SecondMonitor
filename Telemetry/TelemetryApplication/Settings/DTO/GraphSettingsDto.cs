namespace SecondMonitor.Telemetry.TelemetryApplication.Settings.DTO
{
    using System.Xml.Serialization;

    public class GraphSettingsDto
    {
        [XmlAttribute]
        public string Title { get; set; }

        [XmlAttribute]
        public int GraphPriority { get; set; }

        [XmlAttribute]
        public GraphLocationKind GraphLocation { get; set; }
    }
}