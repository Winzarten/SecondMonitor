﻿namespace SecondMonitor.Rating.Common.DataModel
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Player;

    public class ClassRating
    {
        public ClassRating()
        {
            AiRatings = new List<DriverWithoutRating>();
        }

        [XmlAttribute]
        public string ClassName { get; set; }

        public PlayersRating PlayersRating { get; set; }

        public List<DriverWithoutRating> AiRatings { get; set; }
    }
}