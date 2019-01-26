namespace SecondMonitor.Telemetry.TelemetryManagement.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "SessionInfo")]
    public class SessionInfoDto
    {

        [XmlIgnore]
        public DateTime SessionRunDateTime { get; set; }

        [XmlAttribute]
        public string SessionDateTime
        {
            get => SessionRunDateTime.ToString("O");
            set => SessionRunDateTime = DateTime.Parse(value, null, DateTimeStyles.RoundtripKind);
        }

        [XmlAttribute]
        public string Id { get; set; }

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
        public string SessionType { get; set; }

        [XmlAttribute]
        public string CarName { get; set; }

        public List<LapSummaryDto> LapsSummary { get; set; }

    }
}