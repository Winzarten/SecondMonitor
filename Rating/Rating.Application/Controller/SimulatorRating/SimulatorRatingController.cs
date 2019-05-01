namespace SecondMonitor.Rating.Application.Controller.SimulatorRating
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Configuration;
    using Common.DataModel;
    using Common.DataModel.Player;
    using Common.Repository;
    using NLog;
    using RatingProvider;

    public class SimulatorRatingController : ISimulatorRatingController
    {
        private const double GlickoDeviationC = 22.3;
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

        public event EventHandler<RatingChangeArgs> ClassRatingChanged;
        public event EventHandler<RatingChangeArgs> SimulatorRatingChanged;
        public int MinimumAiDifficulty => _simulatorRatingConfiguration.MinimumAiLevel;
        public int MaximumAiDifficulty => _simulatorRatingConfiguration.MaximumAiLevel;

        public double AiTimeDifferencePerLevel => _simulatorRatingConfiguration.AiTimeDifferencePerLevel;

        public int RatingPerLevel => _simulatorRatingConfiguration.RatingPerLevel;

        public int QuickRaceAiRatingForPlace => _simulatorRatingConfiguration.QuickRaceAiRatingForPlace;

        public double AiRatingNoise => _simulatorRatingConfiguration.AiRatingNoise;

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
                    CreationTime = DateTime.Now,
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
                    CreationTime = DateTime.Now,
                }
            };
            _simulatorRating.ClassRatings.Add(classRating);
            Logger.Info($"Created Rating for {className}");
            LogRating(classRating.PlayersRating);
            return  classRating;
        }

        public Task StopControllerAsync()
        {
            _ratingRepository.SaveRatings(_ratings);
            return Task.CompletedTask;
        }

        public DriversRating GetPlayerOverallRating()
        {
            return UpdateDeviation(_simulatorRating.PlayersRating);
        }

        public DriversRating GetPlayerRating(string className)
        {
            ClassRating classRating = _simulatorRating.ClassRatings.FirstOrDefault(x => x.ClassName == className) ?? CreateClassRating(className);
            Logger.Info($"Retreived Players Rating for Class {className}");
            LogRating(classRating.PlayersRating);
            return UpdateDeviation(classRating.PlayersRating);
        }

        public void UpdateRating(DriversRating newClassRating, DriversRating newSimRating, string className)
        {
            ClassRating classRating = _simulatorRating.ClassRatings.FirstOrDefault(x => x.ClassName == className) ?? CreateClassRating(className);
            newClassRating.Rating = Math.Max(newClassRating.Rating, _simulatorRatingConfiguration.MinimumRating);
            newSimRating.Rating = Math.Max(newSimRating.Rating, _simulatorRatingConfiguration.MinimumRating);
            DriversRating oldClassRating = classRating.PlayersRating;
            DriversRating oldSimRating = _simulatorRating.PlayersRating;
            classRating.PlayersRating = newClassRating;
            _simulatorRating.PlayersRating = newSimRating;
            _ratingRepository.SaveRatings(_ratings);
            NotifyRatingsChanges(CreateChangeArgs(oldClassRating, classRating.PlayersRating, className), CreateChangeArgs(oldSimRating, _simulatorRating.PlayersRating, _simulatorName));
        }

        public int GetSuggestedDifficulty(string className)
        {
            DriversRating driversRating = GetPlayerRating(className);
            return Math.Min(_simulatorRatingConfiguration.MinimumAiLevel + ((driversRating.Rating - _simulatorRatingConfiguration.MinimumRating) / _simulatorRatingConfiguration.RatingPerLevel), _simulatorRatingConfiguration.MaximumAiLevel);
        }

        public int GetRatingForDifficulty(int aiDifficulty)
        {
            return (int)(_simulatorRatingConfiguration.MinimumRating + (aiDifficulty - _simulatorRatingConfiguration.MinimumAiLevel + 0.5) * _simulatorRatingConfiguration.RatingPerLevel);
        }

        public IReadOnlyCollection<string> GetAllKnowClassNames()
        {
            return _simulatorRating.ClassRatings.Select(x => x.ClassName).ToList();
        }

        public DriverWithoutRating GetAiRating(string aiDriverName)
        {
            return new DriverWithoutRating()
            {
                Deviation = 50, Volatility = 0.02, Name = aiDriverName
            };
        }

        private static void LogRating(DriversRating driversRating)
        {
            Logger.Info($"Rating - {driversRating.Rating}, Deviation - {driversRating.Deviation}, Volatility - {driversRating.Volatility}");
        }

        private static RatingChangeArgs CreateChangeArgs(DriversRating oldRating, DriversRating newRating, string ratingName)
        {
            return new RatingChangeArgs(newRating, newRating.Rating - oldRating.Rating, newRating.Deviation - oldRating.Deviation, newRating.Volatility - oldRating.Volatility, ratingName);
        }

        private void NotifyRatingsChanges(RatingChangeArgs classRatingChange, RatingChangeArgs simRatingChange)
        {
            ClassRatingChanged?.Invoke(this, classRatingChange);
            SimulatorRatingChanged?.Invoke(this, simRatingChange);
        }

        private static DriversRating UpdateDeviation(DriversRating driversRating)
        {
            int daysOfInactivity = (int)Math.Floor((DateTime.Now - driversRating.CreationTime).TotalDays);
            double newDeviation = driversRating.Deviation;
            for (int i = 0; i < daysOfInactivity; i++)
            {
                newDeviation = Math.Min(350, Math.Sqrt(Math.Pow(newDeviation, 2) + Math.Pow(GlickoDeviationC, 2)));
            }

            driversRating.Deviation = (int) newDeviation;
            return driversRating;
        }
    }
}