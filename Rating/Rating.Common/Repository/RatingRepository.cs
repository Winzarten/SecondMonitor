namespace SecondMonitor.Rating.Common.Repository
{
    using System.IO;
    using System.Xml.Serialization;
    using DataModel;
    using ViewModels.Settings;

    public class RatingRepository : IRatingRepository
    {
        private readonly string _fileName;
        private readonly string _directory;
        private readonly XmlSerializer _xmlSerializer;
        private readonly object _lockObject = new object();

        public RatingRepository(ISettingsProvider settingsProvider)
        {
            _directory = settingsProvider.RatingsRepositoryPath;
            _fileName = Path.Combine(_directory, "Ratings.xml");
            _xmlSerializer = new XmlSerializer(typeof(Ratings));
        }


        private void CheckDirectory()
        {
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
        }

        public Ratings LoadRatingsOrCreateNew()
        {
            lock (_lockObject)
            {
                CheckDirectory();
                if (!File.Exists(_fileName))
                {
                    return new Ratings();
                }

                using (FileStream file = File.Open(_fileName, FileMode.Open))
                {
                    return _xmlSerializer.Deserialize(file) as Ratings;
                }
            }
        }

        public void SaveRatings(Ratings ratings)
        {
            lock (_lockObject)
            {
                CheckDirectory();
                BackupOld();
                using (FileStream file = File.Exists(_fileName) ? File.Open(_fileName, FileMode.Truncate) : File.Create(_fileName))
                {
                    _xmlSerializer.Serialize(file, ratings);
                }
            }
        }

        private void BackupOld()
        {
            for (int i = 9; i >= 0; i--)
            {
                string originalFile = $"{_fileName}.{i}";
                string backupFile = $"{_fileName}.{i+1}";
                if (File.Exists(originalFile))
                {
                    File.Copy(originalFile, backupFile, true);
                }

            }

            if (File.Exists(_fileName))
            {
                File.Copy(_fileName, $"{_fileName}.0", true);
            }
        }
    }
}