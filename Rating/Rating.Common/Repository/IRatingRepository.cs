namespace SecondMonitor.Rating.Common.Repository
{
    using DataModel;

    public interface IRatingRepository
    {
        Ratings LoadRatingsOrCreateNew();

        void SaveRatings(Ratings ratings);
    }
}