namespace SecondMonitor.Rating.Application.Controller.SimulatorRating.RatingUpdater
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
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

        /*public void UpdateRatingsByResults(Dictionary<string, DriversRating> ratingMap, DriversRating simulatorRating, SessionSummary sessionSummary)
        {
            RatingCalculator ratingCalculator = new RatingCalculator();
            Driver playerResult = sessionSummary.Drivers.First(x => x.IsPlayer);
            var glickoRatingsClass = TransformToGlickoPlayers(ratingMap, ratingCalculator);
            var glickoRatingsSim = TransformToGlickoPlayers(ratingMap, ratingCalculator);
            var playersRatingClass = glickoRatingsSim[playerResult.DriverName];
            var playersRatingSim = simulatorRating.ToGlicko(ratingCalculator);

            var resultsClass = new RatingPeriodResults();
            var resultsSim = new RatingPeriodResults();

            foreach (Driver driverResult in sessionSummary.Drivers)
            {
                if (driverResult == playerResult)
                {
                    continue;
                }

                var driverRatingClass = glickoRatingsClass[driverResult.DriverName];
                var driverRatingSimulator = glickoRatingsSim[driverResult.DriverName];
                if (driverResult.FinishingPosition < playerResult.FinishingPosition)
                {
                    resultsClass.AddResult(driverRatingSimulator, playersRatingSim);
                    resultsClass.AddResult(driverRatingClass, playersRatingClass);
                }
                else
                {
                    playersRatingSim
                    resultsClass.AddResult(driverplayersRatingSimatingSimulator, driverRatingSimulator);
                    resultsClass.AddResult(playersRatingClass, driverRatingClass);
                }
            }


            ratingCalculator.UpdateRatings(resultsSim);
            ratingCalculator.UpdateRatings(resultsClass);


            NotifyRatingChange(playersRatingClass, playersRatingSim, playerResult.ClassName);
        }

        public void UpdateRatingsAsLoss(Dictionary<string, DriversRating> ratingMap, DriversRating simulatorRating, Driver player)
        {
            RatingCalculator ratingCalculator = new RatingCalculator();
            var glickoRatingsClass = TransformToGlickoPlayers(ratingMap, ratingCalculator);
            var glickoRatingsSim = TransformToGlickoPlayers(ratingMap, ratingCalculator);
            var playersRatingClass = glickoRatingsSim[player.DriverName];
            var playersRatingSim = simulatorRating.ToGlicko(ratingCalculator);

            var resultsClass = new RatingPeriodResults();
            var resultsSim = new RatingPeriodResults();

            foreach (KeyValuePair<string, Rating> nameRatingPair in glickoRatingsClass)
            {
                if (nameRatingPair.Key == player.DriverName)
                {
                    continue;
                }
                resultsClass.AddResult(nameRatingPair.Value, playersRatingClass);
            }

            foreach (KeyValuePair<string, Rating> nameRatingPair in glickoRatingsSim)
            {
                if (nameRatingPair.Key == player.DriverName)
                {
                    continue;
                }
                resultsSim.AddResult(nameRatingPair.Value, playersRatingSim);
            }

            ratingCalculator.UpdateRatings(resultsSim);
            ratingCalculator.UpdateRatings(resultsClass);


            NotifyRatingChange(playersRatingClass, playersRatingSim, player.ClassName);
        }

        private void NotifyRatingChange(Rating playerClassRating, Rating playerAsSimulator, string className)
        {
            /*var newPlayerClassRating = GlickoCalculator.CalculateRanking(player, opponents);
            var newPlayerSimRatingRating = GlickoCalculator.CalculateRanking(playerAsSimulator, opponents);#1#
            _ratingController.UpdateRating(playerClassRating.FromGlicko(), playerAsSimulator.FromGlicko(), className);
        }

        private static Dictionary<string, Rating> TransformToGlickoPlayers(Dictionary<string, DriversRating> ratings, RatingCalculator ratingCalculator)
        {
            return ratings.Select(x => new KeyValuePair<string, Rating>(x.Key, x.Value.ToGlicko(ratingCalculator))).ToDictionary(x => x.Key, x => x.Value);
        }*/

        public void UpdateRatingsByResults(Dictionary<string, DriversRating> ratings, DriversRating simulatorRating, SessionSummary sessionSummary)
        {
            Driver player = sessionSummary.Drivers.First(x => x.IsPlayer);
            Driver[] eligibleDrivers = sessionSummary.Drivers.Where(x => !x.IsPlayer && ratings.ContainsKey(x.DriverName)).ToArray();
            var glickoRatingsClass = TransformToGlickoPlayers(ratings);
            var glickoRatingsSim = TransformToGlickoPlayers(ratings);
            var playerRating = glickoRatingsClass[player.DriverName];
            var playerSimRating = simulatorRating.ToGlicko(playerRating.Name);
            var opponentsClass = eligibleDrivers.Select(x => new GlickoOpponent(glickoRatingsClass[x.DriverName], x.FinishingPosition < player.FinishingPosition ? 0 : 1)).ToList();
            var opponentSim = eligibleDrivers.Select(x => new GlickoOpponent(glickoRatingsSim[x.DriverName], x.FinishingPosition < player.FinishingPosition ? 0 : 1)).ToList();
            ComputeNewRatingsAndNotify(playerRating, playerSimRating, opponentsClass,  opponentSim, player.ClassName, sessionSummary.TrackInfo.TrackFullName);
        }

        public void UpdateRatingsAsLoss(Dictionary<string, DriversRating> ratings, DriversRating simulatorRating, Driver player, string trackName)
        {
            var glickoRatings = TransformToGlickoPlayers(ratings);
            var playerRating = glickoRatings[player.DriverName];
            var playerSimRating = simulatorRating.ToGlicko(playerRating.Name);
            var opponents = glickoRatings.Where(x => x.Key != player.DriverName).Select(x => new GlickoOpponent(x.Value, 0)).ToList();
            ComputeNewRatingsAndNotify(playerRating, playerSimRating, opponents, opponents, player.ClassName, trackName);
        }

        private void ComputeNewRatingsAndNotify(GlickoPlayer player, GlickoPlayer playerAsSimulator, List<GlickoOpponent> opponentsClass, List<GlickoOpponent> opponentsSim, string className, string trackName)
        {
            var newPlayerClassRating = GlickoCalculator.CalculateRanking(player, opponentsClass);
            var newPlayerSimRatingRating = GlickoCalculator.CalculateRanking(playerAsSimulator, opponentsSim);
            _ratingController.UpdateRating(newPlayerClassRating.FromGlicko(), newPlayerSimRatingRating.FromGlicko(), className, trackName);
        }

        private static Dictionary<string, GlickoPlayer> TransformToGlickoPlayers(Dictionary<string, DriversRating> ratings)
        {
            return ratings.Select(x => new KeyValuePair<string, GlickoPlayer>(x.Key, x.Value.ToGlicko(x.Key))).ToDictionary(x => x.Key, x=> x.Value);
        }
    }
}