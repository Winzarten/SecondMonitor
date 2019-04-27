namespace SecondMonitor.Rating.Application.Controller.SimulatorRating
{
    using System.Threading.Tasks;
    using Common.Repository;

    public class RatingStorageController : IRatingStorageController
    {
        private readonly IRatingRepository _ratingRepository;

        public RatingStorageController(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public Task StartControllerAsync()
        {
            return Task.CompletedTask;
        }

        public Task StopControllerAsync()
        {
            return Task.CompletedTask;
        }
    }
}