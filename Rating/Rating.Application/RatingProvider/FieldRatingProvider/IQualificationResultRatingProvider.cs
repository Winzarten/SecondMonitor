namespace SecondMonitor.Rating.Application.RatingProvider.FieldRatingProvider
{
    using System.Collections.Generic;
    using Common.DataModel.Player;
    using DataModel.Snapshot.Drivers;
    using DataModel.Summary;

    public interface IQualificationResultRatingProvider
    {
        Dictionary<string, DriversRating> CreateFieldRatingFromQualificationResult(List<Driver> qualificationResult, int difficulty);
        Dictionary<string, DriversRating> CreateFieldRating(DriverInfo[] fieldDrivers, int difficulty);
    }
}