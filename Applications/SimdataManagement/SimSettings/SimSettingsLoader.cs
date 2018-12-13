using System;
using System.Xml;

namespace SecondMonitor.SimdataManagement.SimSettings
{
    using DataModel.OperationalRange;
    using System.IO;
    using System.Xml.Serialization;

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

            using (FileStream file = File.Exists(path) ? File.Open(path, FileMode.Truncate) : File.Create(path))
            {
                _xmlSerializer.Serialize(file, properties);
            }
        }

        private DataSourceProperties LoadDataSourceProperties(string filePath)
        {
            try
            {
                using (TextReader file = File.OpenText(filePath))
                {

                    XmlReader reader = XmlReader.Create(file, new XmlReaderSettings() { CheckCharacters = false });
                    var foo = (DataSourceProperties)_xmlSerializer.Deserialize(reader);
                    return foo;
                }

            }
            catch (Exception ex)
            {
                string copiedFilePath = ResetInvalidFile(filePath);
                throw new SimSettingsException($"Error in configuration file : {filePath}. File was recreated, but all car settings for sim were lost. Corrupted file was copied to {copiedFilePath}. ", ex);
            }
        }

        private static string ResetInvalidFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }

            string newFileName = filePath + ".error";
            File.Move(filePath, newFileName);
            return newFileName;
        }
    }
}