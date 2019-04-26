namespace SecondMonitor.Rating.Application.Controller
{
    using System.Threading.Tasks;
    using Common.Repository;
    using DataModel.Snapshot;
    using DataModel.Summary;
    using SecondMonitor.ViewModels.Factory;
    using ViewModels;

    public class RatingApplicationController : IRatingApplicationController
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IViewModelFactory _viewModelFactory;

        public RatingApplicationController(IRatingRepository ratingRepository, IViewModelFactory viewModelFactory)
        {
            _ratingRepository = ratingRepository;
            _viewModelFactory = viewModelFactory;
        }

        public IRatingApplicationViewModel RatingApplicationViewModel { get; set; }

        public Task StartControllerAsync()
        {
            RatingApplicationViewModel = _viewModelFactory.Create<IRatingApplicationViewModel>();
            return Task.CompletedTask;
        }

        public Task StopControllerAsync()
        {
            return Task.CompletedTask;
        }

        public void NotifySessionCompletion(SessionSummary sessionSummary)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyDataLoaded(SimulatorDataSet simulatorDataSet)
        {
            throw new System.NotImplementedException();
        }
    }
}