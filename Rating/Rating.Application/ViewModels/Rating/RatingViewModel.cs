namespace SecondMonitor.Rating.Application.ViewModels.Rating
{
    using Common.DataModel.Player;
    using SecondMonitor.ViewModels;

    public class RatingViewModel : AbstractViewModel<DriversRating>, IRatingViewModel
    {
        private string _secondaryRating;
        private int _ratingChange;
        private string _mainRating;
        private bool _ratingChangeVisible;

        public string SecondaryRating
        {
            get => _secondaryRating;
            set => SetProperty(ref _secondaryRating, value);
        }

        public int RatingChange
        {
            get => _ratingChange;
            set => SetProperty(ref _ratingChange, value);
        }

        public bool RatingChangeVisible
        {
            get => _ratingChangeVisible;
            set => SetProperty(ref _ratingChangeVisible, value);
        }

        public string MainRating
        {
            get => _mainRating;
            set => SetProperty(ref _mainRating, value);
        }

        protected override void ApplyModel(DriversRating model)
        {
            MainRating = model.Rating.ToString();
            SecondaryRating = $"{model.Deviation}-{model.Volatility:F2}";
        }

        public override DriversRating SaveToNewModel()
        {
            throw new System.NotImplementedException();
        }
    }
}