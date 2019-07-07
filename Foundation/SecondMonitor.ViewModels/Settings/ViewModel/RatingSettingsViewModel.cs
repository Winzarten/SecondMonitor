namespace SecondMonitor.ViewModels.Settings.ViewModel
{
    using Model;

    public class RatingSettingsViewModel : AbstractViewModel<RatingSettings>
    {
        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        protected override void ApplyModel(RatingSettings model)
        {
            IsEnabled = model.IsEnabled;
        }

        public override RatingSettings SaveToNewModel()
        {
            return new RatingSettings()
            {
                IsEnabled = IsEnabled,
            };
        }
    }
}