namespace SecondMonitor.Rating.Application.ViewModels
{
    using Common.DataModel.Player;
    using Rating;
    using SecondMonitor.ViewModels;
    using SecondMonitor.ViewModels.Factory;

    public class RatingApplicationViewModel : AbstractViewModel, IRatingApplicationViewModel
    {
        private IRatingViewModel _simulatorRating;
        private IRatingViewModel _classRating;

        public RatingApplicationViewModel(IViewModelFactory viewModelFactory)
        {
            SimulatorRating = viewModelFactory.Create<IRatingViewModel>();
            ClassRating = viewModelFactory.Create<IRatingViewModel>();
        }

        public IRatingViewModel SimulatorRating
        {
            get => _simulatorRating;
            private set => SetProperty(ref _simulatorRating, value);
        }

        public IRatingViewModel ClassRating
        {
            get => _classRating;
            private set => SetProperty(ref _classRating, value);
        }

        public void ApplySimulatorRating(DriversRating driversRating)
        {
            SimulatorRating.FromModel(driversRating);
        }

        public void ApplyClassRating(DriversRating driversRating)
        {
            ClassRating.FromModel(driversRating);
        }
    }
}