namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States.Context
{
    using System.Collections.Generic;
    using Common.DataModel.Player;

    public class RaceContext
    {
        public int UsedDifficulty { get; set; }

        public Dictionary<string, DriversRating> FieldRating { get; set; }
    }
}