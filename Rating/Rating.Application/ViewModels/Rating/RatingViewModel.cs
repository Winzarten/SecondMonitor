namespace SecondMonitor.Rating.Application.ViewModels.Rating
{
    using Common.DataModel.Player;
    using SecondMonitor.ViewModels;

    public class RatingViewModel : AbstractViewModel<DriversRating>, IRatingViewModel
    {
        private string _secondaryRating;
        private string _ratingChange;
        private string _mainRating;

        public string SecondaryRating
        {
            get => _secondaryRating;
            set => SetProperty(ref _secondaryRating, value);
        }

        public string RatingChange
        {
            get => _ratingChange;
            set => SetProperty(ref _ratingChange, value);
        }

        public string MainRating
        {
            get => _mainRating;
            set => SetProperty(ref _mainRating, value);
        }

        protected override void ApplyModel(DriversRating model)
        {
            MainRating = model.Rating.ToString();
            SecondaryRating = $"{model.Deviation}-{model.Volatility}";
        }

        public void ApplyRatingChange(int ratingChange)
        {
            if (ratingChange == 0)
            {
                RatingChange = string.Empty;
            }

            RatingChange = ratingChange < 0 ? $"-{ratingChange}" : $"+{ratingChange}";
        }

        public override DriversRating SaveToNewModel()
        {
            throw new System.NotImplementedException();
        }
    }
}