namespace SecondMonitor.Rating.Application.RatingProvider
{
    using System;
    using Common.DataModel.Player;

    public class RatingChangeArgs : EventArgs
    {
        public RatingChangeArgs(DriversRating newRating, int ratingChange, int deviationChange, double volatilityChange, string ratingName)
        {
            NewRating = newRating;
            RatingChange = ratingChange;
            DeviationChange = deviationChange;
            VolatilityChange = volatilityChange;
            RatingName = ratingName;
        }

        public DriversRating NewRating { get; }
        public int RatingChange { get; }
        public int DeviationChange { get; }
        public double VolatilityChange { get; }
        public string RatingName { get; }
    }
}