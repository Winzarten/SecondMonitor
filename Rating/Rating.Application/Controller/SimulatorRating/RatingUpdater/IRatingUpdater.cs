namespace SecondMonitor.Rating.Application.Controller.SimulatorRating.RatingUpdater
{
    using System.Collections.Generic;
    using Common.DataModel.Player;
    using DataModel.Summary;

    public interface IRatingUpdater
    {
        void UpdateRatingsByResults(Dictionary<string, DriversRating> ratingMap, DriversRating simulatorRating, SessionSummary sessionSummary);
        void UpdateRatingsAsLoss(Dictionary<string, DriversRating> ratingMap, DriversRating simulatorRating, Driver player, string trackName);
    }
}