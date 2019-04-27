namespace SecondMonitor.Rating.Application.Controller.SimulatorRating
{
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Configuration;
    using Common.DataModel;
    using Common.DataModel.Player;
    using Common.Repository;

    public class SimulatorRatingController : ISimulatorRatingController
    {
        private readonly string _simulatorName;
        private readonly IRatingRepository _ratingRepository;
        private readonly SimulatorRatingConfiguration _simulatorRatingConfiguration;
        private Ratings _ratings;
        private SimulatorRating _simulatorRating;

        public SimulatorRatingController(string simulatorName, IRatingRepository ratingRepository, ISimulatorRatingConfigurationProvider simulatorRatingConfigurationProvider)
        {
            _simulatorName = simulatorName;
            _ratingRepository = ratingRepository;
            _simulatorRatingConfiguration = simulatorRatingConfigurationProvider.GetRatingConfiguration(simulatorName);
        }

        public Task StartControllerAsync()
        {
            _ratings = _ratingRepository.LoadRatingsOrCreateNew();
            _simulatorRating = _ratings.SimulatorsRatings.FirstOrDefault(x => x.SimulatorName == _simulatorName);
            if (_simulatorRating != null)
            {
                return Task.CompletedTask;
            }

            _simulatorRating = CreateSimulatorRating();
            _ratings.SimulatorsRatings.Add(_simulatorRating);

            return Task.CompletedTask;
        }

        private SimulatorRating CreateSimulatorRating()
        {
            SimulatorRating simulatorRating = new SimulatorRating()
            {
                SimulatorName = _simulatorName,
                PlayersRating = new DriversRating()
                {
                    Rating = _simulatorRatingConfiguration.DefaultPlayerRating,
                    Deviation = _simulatorRatingConfiguration.DefaultPlayerDeviation,
                    Volatility = _simulatorRatingConfiguration.DefaultPlayerVolatility,
                }
            };
            return simulatorRating;
        }

        public Task StopControllerAsync()
        {
            _ratingRepository.SaveRatings(_ratings);
            return Task.CompletedTask;
        }

        public DriversRating GetPlayerOverallRating()
        {
            return _simulatorRating.PlayersRating;
        }

        public DriversRating GetPlayerRating(string className)
        {
            throw new System.NotImplementedException();
        }

        public DriverWithoutRating GetAiRating(string aiDriverName, string className)
        {
            throw new System.NotImplementedException();
        }
    }
}