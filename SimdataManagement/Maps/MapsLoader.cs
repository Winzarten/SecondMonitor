namespace SecondMonitor.SimdataManagement
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using DataModel.TrackMap;

    public class MapsLoader
    {
        private const string FileSuffix = ".xml";

        private readonly XmlSerializer _xmlSerializer;

        public MapsLoader(string mapsPath)
        {
            MapsPath = mapsPath;
            _xmlSerializer = new XmlSerializer(typeof(TrackMapDto));
        }

        public bool TryLoadMap(string simulator, string trackName, out TrackMapDto trackMapDto)
        {
            string fullPathName = Path.Combine(Path.Combine(MapsPath, simulator), trackName + FileSuffix);
            try
            {
                using (TextReader file = File.OpenText(fullPathName))
                {

                    XmlReader reader = XmlReader.Create(file, new XmlReaderSettings() { CheckCharacters = false });
                    trackMapDto = (TrackMapDto)_xmlSerializer.Deserialize(reader);
                }
                return true;
            }
            catch (Exception)
            {
                if (File.Exists(fullPathName))
                {
                    File.Delete(fullPathName);
                }

                trackMapDto = null;
                return false;
            }
        }

        public void SaveMap(string simulator, string trackName, TrackMapDto trackMapDto)
        {
            string directory = Path.Combine(MapsPath, simulator);
            Directory.CreateDirectory(directory);
            string fullPathName = Path.Combine(directory, trackName + FileSuffix);
            using (FileStream file = File.Exists(fullPathName) ? File.Open(fullPathName, FileMode.Truncate) : File.Create(fullPathName))
            {
                _xmlSerializer.Serialize(file, trackMapDto);
            }
        }

        public void RemoveMap(string simulator, string trackName)
        {
            string directory = Path.Combine(MapsPath, simulator);
            string fullPathName = Path.Combine(directory, trackName + FileSuffix);
            if (File.Exists(fullPathName))
            {
                File.Delete(fullPathName);
            }
        }

        public string MapsPath { get; }
    }
}