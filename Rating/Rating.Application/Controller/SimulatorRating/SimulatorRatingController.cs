namespace SecondMonitor.Rating.Application.Controller.SimulatorRating
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Configuration;
    using Common.DataModel;
    using Common.DataModel.Player;
    using Common.Repository;
    using NLog;

    public class SimulatorRatingController : ISimulatorRatingController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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

            Logger.Info($"Created Default Simulator Rating for {_simulatorName}");
            LogRating(simulatorRating.PlayersRating);
            return simulatorRating;
        }

        private ClassRating CreateClassRating(string className)
        {
            ClassRating classRating = new ClassRating()
            {
                ClassName = className,
                PlayersRating = new DriversRating()
                {
                    Rating = _simulatorRating.PlayersRating.Rating,
                    Deviation = _simulatorRating.PlayersRating.Deviation,
                    Volatility = _simulatorRating.PlayersRating.Volatility,
                }
            };
            _simulatorRating.ClassRatings.Add(classRating);
            Logger.Info($"Created Rating for {className}");
            LogRating(classRating.PlayersRating);
            return classRating;
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
            ClassRating classRating = _simulatorRating.ClassRatings.FirstOrDefault(x => x.ClassName == className) ?? CreateClassRating(className);
            Logger.Info($"Retreived Players Rating for Class {className}");
            LogRating(classRating.PlayersRating);
            return classRating.PlayersRating;
        }

        public IReadOnlyCollection<string> GetAllKnowClassNames()
        {
            return _simulatorRating.ClassRatings.Select(x => x.ClassName).ToList();
        }

        public DriverWithoutRating GetAiRating(string aiDriverName, string className)
        {
            throw new System.NotImplementedException();
        }

        private static void LogRating(DriversRating driversRating)
        {
            Logger.Info($"Rating - {driversRating.Rating}, Deviation - {driversRating.Deviation}, Volatility - {driversRating.Volatility}");
        }
    }
}