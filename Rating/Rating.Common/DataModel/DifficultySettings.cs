namespace SecondMonitor.Rating.Common.DataModel
{
    using System.Xml.Serialization;

    public class DifficultySettings
    {
        [XmlAttribute]
        public int SelectedDifficulty { get; set; }

        [XmlAttribute]
        public bool WasUserSelected { get; set; }
    }
}