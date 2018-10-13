namespace SecondMonitor.SimdataManagement.SimSettings
{
    using System.IO;
    using System.Xml.Serialization;

    using DataModel.OperationalRange;

    public class SimSettingsLoader
    {
        private const string FileSuffix = ".xml";

        private readonly XmlSerializer _xmlSerializer;

        public SimSettingsLoader(string primaryPath, string overridingPath)
        {
            PrimaryPath = primaryPath;
            OverridingPath = overridingPath;
            _xmlSerializer = new XmlSerializer(typeof(DataSourceProperties));
        }

        public string PrimaryPath { get; }
        public string OverridingPath { get; set; }

        public DataSourceProperties GetDataSourcePropertiesAsync(string sourceName)
        {
            string primaryFilePath = Path.Combine(PrimaryPath, sourceName + FileSuffix);
            DataSourceProperties baseProperties = File.Exists(primaryFilePath) ? LoadDataSourceProperties(primaryFilePath) : new DataSourceProperties() { SourceName = sourceName };

            string secondaryPath = Path.Combine(OverridingPath, sourceName + FileSuffix);
            if (File.Exists(secondaryPath))
            {
                baseProperties.OverrideWith(LoadDataSourceProperties(secondaryPath));
            }

            return baseProperties;
        }

        public void SaveDataSourceProperties(DataSourceProperties properties)
        {
            Directory.CreateDirectory(OverridingPath);
            string path = Path.Combine(OverridingPath, properties.SourceName + FileSuffix);
            using (FileStream file = File.OpenWrite(path))
            {
                _xmlSerializer.Serialize(file, properties);
            }
        }

        private DataSourceProperties LoadDataSourceProperties(string filePath)
        {
            using (StreamReader file = File.OpenText(filePath))
            {
                return (DataSourceProperties) _xmlSerializer.Deserialize(file);
            }
        }
    }
}