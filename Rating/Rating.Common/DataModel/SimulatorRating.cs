namespace SecondMonitor.Rating.Common.DataModel
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Player;

    public class SimulatorRating
    {
        public SimulatorRating()
        {
            ClassRatings = new List<ClassRating>();
            RunTracks = new HashSet<string>();
        }

        [XmlAttribute]
        public string SimulatorName { get; set; }

        public DriversRating PlayersRating { get; set; }

        public List<ClassRating> ClassRatings { get; set; }

        public HashSet<string> RunTracks { get; set; }
    }
}