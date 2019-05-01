namespace SecondMonitor.Rating.Application.Controller.SimulatorRating.RatingUpdater
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.DataModel.Player;
    using DataModel.Summary;
    using Glicko2;

    public class RatingUpdater : IRatingUpdater
    {
        private readonly ISimulatorRatingController _ratingController;

        public RatingUpdater(ISimulatorRatingController ratingController)
        {
            _ratingController = ratingController;
        }

        public void UpdateRatingsByResults(Dictionary<string, DriversRating> ratings, DriversRating simulatorRating, SessionSummary sessionSummary)
        {
            Driver player = sessionSummary.Drivers.First(x => x.IsPlayer);
            Driver[] eligibleDrivers = sessionSummary.Drivers.Where(x => !x.IsPlayer && ratings.ContainsKey(x.DriverName)).ToArray();
            var glickoRatings = TransformToGlickoPlayers(ratings);
            var playerRating = glickoRatings[player.DriverName];
            var playerSimRating = simulatorRating.ToGlicko(playerRating.Name);
            var opponents = eligibleDrivers.Select(x => new GlickoOpponent(glickoRatings[x.DriverName], x.FinishingPosition < player.FinishingPosition ? 0 : 1)).ToList();
            ComputeNewRatingsAndNotify(playerRating, playerSimRating, opponents, player.ClassName);
        }

        public void UpdateRatingsAsLoss(Dictionary<string, DriversRating> ratings, DriversRating simulatorRating, Driver player)
        {
            var glickoRatings = TransformToGlickoPlayers(ratings);
            var playerRating = glickoRatings[player.DriverName];
            var playerSimRating = simulatorRating.ToGlicko(playerRating.Name);
            var opponents = glickoRatings.Where(x => x.Key != player.DriverName).Select(x => new GlickoOpponent(x.Value, 0)).ToList();
            ComputeNewRatingsAndNotify(playerRating, playerSimRating, opponents, player.ClassName);
        }

        private void ComputeNewRatingsAndNotify(GlickoPlayer player, GlickoPlayer playerAsSimulator, List<GlickoOpponent> opponents, string className)
        {
            var newPlayerClassRating = GlickoCalculator.CalculateRanking(player, opponents);
            var newPlayerSimRatingRating = GlickoCalculator.CalculateRanking(playerAsSimulator, opponents);
            _ratingController.UpdateRating(newPlayerClassRating.FromGlicko(), newPlayerSimRatingRating.FromGlicko(), className);
        }

        private static Dictionary<string, GlickoPlayer> TransformToGlickoPlayers(Dictionary<string, DriversRating> ratings)
        {
            return ratings.Select(x => new KeyValuePair<string, GlickoPlayer>(x.Key, x.Value.ToGlicko(x.Key))).ToDictionary(x => x.Key, x=> x.Value);
        }
    }
}