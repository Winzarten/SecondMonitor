namespace SecondMonitor.Rating.Application.RatingProvider.FieldRatingProvider
{
    using System.Collections.Generic;
    using Common.DataModel.Player;
    using DataModel.Summary;

    public interface IQualificationResultRatingProvider
    {
        Dictionary<string, DriversRating> CreateFieldRating(List<Driver> qualificationResult, int difficulty);
    }
}