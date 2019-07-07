namespace SecondMonitor.Rating.Application.RatingProvider
{
    using Common.DataModel.Player;

    public interface IRatingProvider
    {
        bool  TryGetRatingForDriverCurrentSession(string driverName, out DriversRating driversRating);
    }
}