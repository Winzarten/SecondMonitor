namespace SecondMonitor.PluginsConfiguration.Common.DataModel
{
    using System.Xml.Serialization;

    public class RemoteConfiguration
    {
        [XmlAttribute]
        public string IpAddress { get; set; }

        [XmlAttribute]
        public bool IsFindInLanEnabled { get; set; }

        [XmlAttribute]
        public int Port { get; set; }

        [XmlAttribute]
        public bool IsRemoteConnectorEnabled { get; set; }
    }
}