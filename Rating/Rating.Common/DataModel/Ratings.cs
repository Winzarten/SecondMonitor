namespace SecondMonitor.Rating.Common.DataModel
{
    using System.Collections.Generic;

    public class Ratings
    {
        public Ratings()
        {
            SimulatorsRatings = new List<SimulatorRating>();
        }

        public List<SimulatorRating> SimulatorsRatings { get; set; }
    }
}