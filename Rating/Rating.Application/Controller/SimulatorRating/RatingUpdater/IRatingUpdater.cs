namespace SecondMonitor.Rating.Application.Controller.SimulatorRating.RatingUpdater
{
    using System.Collections.Generic;
    using Common.DataModel.Player;
    using DataModel.Summary;

    public interface IRatingUpdater
    {
        void UpdateRatingsByResults(Dictionary<string, DriversRating> ratings, DriversRating simulatorRating, SessionSummary sessionSummary);
        void UpdateRatingsAsLoss(Dictionary<string, DriversRating> ratings, DriversRating simulatorRating, Driver player);
    }
}