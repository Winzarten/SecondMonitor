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
        private readonly IRatingRepository _ratingRepository;
        private readonly SimulatorRatingConfiguration _simulatorRatingConfiguration;
        private Ratings _ratings;
        private SimulatorRating _simulatorRating;
        private readonly Random _random;

        public SimulatorRatingController(string simulatorName, IRatingRepository ratingRepository, ISimulatorRatingConfigurationProvider simulatorRatingConfigurationProvider)
        {
            _random = new Random();
            SimulatorName = simulatorName;
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

        public string SimulatorName { get; }

        public double AiRatingNoise => _simulatorRatingConfiguration.AiRatingNoise;

        public Task StartControllerAsync()
        {
            _ratings = _ratingRepository.LoadRatingsOrCreateNew();
            _simulatorRating = _ratings.SimulatorsRatings.FirstOrDefault(x => x.SimulatorName == SimulatorName);
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
                SimulatorName = SimulatorName,
                PlayersRating = new DriversRating()
                {
                    Rating = _simulatorRatingConfiguration.DefaultPlayerRating,
                    Deviation = _simulatorRatingConfiguration.DefaultPlayerDeviation,
                    Volatility = _simulatorRatingConfiguration.DefaultPlayerVolatility,
                    CreationTime = DateTime.Now,
                }
            };

            Logger.Info($"Created Default Simulator Rating for {SimulatorName}");
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
                },
                DifficultySettings = new DifficultySettings()
                {
                    SelectedDifficulty = GetSuggestedDifficulty(_simulatorRating.PlayersRating.Rating),
                    WasUserSelected =  true,
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

        public void SetSelectedDifficulty(int difficulty, bool wasUserSelected, string className)
        {
            ClassRating classRating = GetOrCreateClassRating(className);
            classRating.DifficultySettings = new DifficultySettings()
            {
                SelectedDifficulty = difficulty,
                WasUserSelected = wasUserSelected
            };
        }

        public void UpdateRating(DriversRating newClassRating, DriversRating newSimRating, string className, string trackName)
        {
            ClassRating classRating = GetOrCreateClassRating(className);
            DriversRating oldClassRating = classRating.PlayersRating;
            DriversRating oldSimRating = _simulatorRating.PlayersRating;
            newSimRating = NormalizeRatingChange(oldSimRating, newSimRating);
            newClassRating = NormalizeRatingChange(oldClassRating, newClassRating);
            classRating.PlayersRating = newClassRating;
            _simulatorRating.PlayersRating = newSimRating;
            _simulatorRating.RunTracks.Add(trackName);

            if (!classRating.DifficultySettings.WasUserSelected)
            {
                classRating.DifficultySettings.SelectedDifficulty = GetSuggestedDifficulty(className);
            }

            _ratingRepository.SaveRatings(_ratings);
            NotifyRatingsChanges(CreateChangeArgs(oldClassRating, classRating.PlayersRating, className), CreateChangeArgs(oldSimRating, _simulatorRating.PlayersRating, SimulatorName));
        }

        public string GetRaceSuggestion()
        {
            if (_simulatorRating.ClassRatings.Count == 0 || _simulatorRating.RunTracks.Count == 0)
            {
                return string.Empty;
            }

            List<string> runTracks = _simulatorRating.RunTracks.ToList();
            return $"{_simulatorRating.ClassRatings[_random.Next(_simulatorRating.ClassRatings.Count)].ClassName}, at: {runTracks[_random.Next(runTracks.Count)]}";
        }


        public int GetSuggestedDifficulty(string className)
        {
            DriversRating driversRating = GetPlayerRating(className);
            return GetSuggestedDifficulty(driversRating.Rating);
        }

        public int GetSuggestedDifficulty(int rating)
        {
            return Math.Min(_simulatorRatingConfiguration.MinimumAiLevel + ((rating - _simulatorRatingConfiguration.MinimumRating) / _simulatorRatingConfiguration.RatingPerLevel), _simulatorRatingConfiguration.MaximumAiLevel);
        }

        public DifficultySettings GetDifficultySettings(string className)
        {
            return GetOrCreateClassRating(className).DifficultySettings;
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
                Deviation = 100, Volatility = 0.06, Name = aiDriverName
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

        private ClassRating GetOrCreateClassRating(string className)
        {
            return _simulatorRating.ClassRatings.FirstOrDefault(x => x.ClassName == className) ?? CreateClassRating(className);
        }

        private void NotifyRatingsChanges(RatingChangeArgs classRatingChange, RatingChangeArgs simRatingChange)
        {
            Logger.Info("New Simulator Rating:");
            LogRating(simRatingChange.NewRating);
            Logger.Info("New Class Rating:");
            LogRating(classRatingChange.NewRating);
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

        private DriversRating NormalizeRatingChange(DriversRating oldRating, DriversRating newRating)
        {
            newRating.Rating = Math.Max(newRating.Rating, _simulatorRatingConfiguration.MinimumRating);
            int maximumChange = _simulatorRatingConfiguration.RatingPerLevel * 5;
            int ratingDifference = newRating.Rating - oldRating.Rating;
            if (Math.Abs(ratingDifference) > maximumChange)
            {
                newRating.Rating = ratingDifference < 0 ? oldRating.Rating - maximumChange : oldRating.Rating + maximumChange;
            }

            return newRating;
        }
    }
}