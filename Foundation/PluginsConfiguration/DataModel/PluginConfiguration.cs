namespace SecondMonitor.PluginsConfiguration.DataModel
{
    using System.Xml.Serialization;

    public class PluginConfiguration
    {
        [XmlAttribute]
        public string PluginName { get; set; }

        [XmlAttribute]
        public bool IsEnabled { get; set; }
    }
}