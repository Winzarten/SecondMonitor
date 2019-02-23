namespace SecondMonitor.PluginsConfiguration.Common.DataModel
{
    using System.Xml.Serialization;

    public class RemoteConfiguration
    {
        public RemoteConfiguration()
        {
            BroadcastLimitSettings = new BroadcastLimitSettings()
            {
                IsEnabled = false,
                MinimumPackageInterval = 30,
                PlayerTimingPackageInterval = 200,
                OtherDriversTimingPackageInterval = 1000,
            };
        }

        [XmlAttribute]
        public string IpAddress { get; set; }

        [XmlAttribute]
        public bool IsFindInLanEnabled { get; set; }

        [XmlAttribute]
        public int Port { get; set; }

        [XmlAttribute]
        public bool IsRemoteConnectorEnabled { get; set; }

        public BroadcastLimitSettings BroadcastLimitSettings { get; set; }
    }
}