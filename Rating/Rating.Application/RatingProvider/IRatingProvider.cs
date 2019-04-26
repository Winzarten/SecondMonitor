namespace SecondMonitor.Rating.Application.RatingProvider
{
    using System;
    using Common.DataModel.Player;

    public interface IRatingProvider
    {
        event EventHandler<RatingChangeArgs> PlayersRatingChanged;
        DriversRating GetRatingForDriverCurrentSession(string driverName);
    }
}