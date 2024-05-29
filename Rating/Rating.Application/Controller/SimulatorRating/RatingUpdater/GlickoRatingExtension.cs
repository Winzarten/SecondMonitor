namespace SecondMonitor.Rating.Application.Controller.SimulatorRating.RatingUpdater
{
    using System;
    using Common.DataModel.Player;
    using Glicko2;

    public static class GlickoRatingExtension
    {
        public static GlickoPlayer ToGlicko(this DriversRating driversRating, string name)
        {
            return new GlickoPlayer(driversRating.Rating, driversRating.Deviation, driversRating.Volatility) { Name = name};
        }

        public static DriversRating FromGlicko(this GlickoPlayer glickoRating)
        {
            return new DriversRating()
            {
                Rating = (int) glickoRating.Rating,
                Deviation = (int) glickoRating.RatingDeviation,
                Volatility = glickoRating.Volatility,
                CreationTime = DateTime.Now,
            };
        }
    }
}