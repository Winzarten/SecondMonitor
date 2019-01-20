using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using SecondMonitor.DataModel.DriversPresentation;

namespace SecondMonitor.SimdataManagement.DriverPresentation
{
    public class DriverPresentationsLoader
    {
        private readonly string _storedDirectoryPath;
        private const string FileName = "DriverPresentations.xml";

        private readonly XmlSerializer _xmlSerializer;

        public DriverPresentationsLoader(string storedDirectoryPath)
        {
            _storedDirectoryPath = storedDirectoryPath;
            _xmlSerializer = new XmlSerializer(typeof(DriverPresentationsDto));
        }

        public bool TryLoad(out DriverPresentationsDto driverPresentationsDto)
        {
            string fullPathName = Path.Combine(_storedDirectoryPath, FileName);
            try
            {
                using (TextReader file = File.OpenText(fullPathName))
                {
                    XmlReader reader = XmlReader.Create(file, new XmlReaderSettings() { CheckCharacters = false });
                    driverPresentationsDto = (DriverPresentationsDto)_xmlSerializer.Deserialize(reader);
                }
                return true;
            }
            catch (Exception)
            {
                if (File.Exists(fullPathName))
                {
                    File.Delete(fullPathName);
                }

                driverPresentationsDto = null;
                return false;
            }
        }

        public void Save(DriverPresentationsDto driverPresentationsDto)
        {
            string fullPathName = Path.Combine(_storedDirectoryPath, FileName);
            Directory.CreateDirectory(_storedDirectoryPath);
            using (FileStream file = File.Exists(fullPathName) ? File.Open(fullPathName, FileMode.Truncate) : File.Create(fullPathName))
            {
                _xmlSerializer.Serialize(file, driverPresentationsDto);
            }
        }
    }
}