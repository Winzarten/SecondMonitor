namespace SecondMonitor.Rating.Application.RatingProvider
{
    using System;
    using Common.DataModel.Player;

    public class RatingChangeArgs : EventArgs
    {
        public RatingChangeArgs(DriversRating newRating, double ratingChange, double deviationChange, double volatilityChange)
        {
            NewRating = newRating;
            RatingChange = ratingChange;
            DeviationChange = deviationChange;
            VolatilityChange = volatilityChange;
        }

        public DriversRating NewRating { get; }
        public double RatingChange { get; }
        public double DeviationChange { get; }
        public double VolatilityChange { get; }
    }
}