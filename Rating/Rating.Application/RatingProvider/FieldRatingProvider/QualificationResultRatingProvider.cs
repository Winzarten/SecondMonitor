namespace SecondMonitor.Rating.Application.RatingProvider.FieldRatingProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.DataModel.Player;
    using Controller.SimulatorRating;
    using DataModel.Summary;

    public class QualificationResultRatingProvider : IQualificationResultRatingProvider
    {
        private readonly ISimulatorRatingController _simulatorRatingController;
        private int _maxNoise;
        private readonly Random _random;
        private TimeSpan _referenceTime;
        private int _referenceRating;

        public QualificationResultRatingProvider(ISimulatorRatingController simulatorRatingController)
        {
            _random = new Random();
            _simulatorRatingController = simulatorRatingController;
        }

        public Dictionary<string, DriversRating> CreateFieldRating(List<Driver> qualificationResult, int difficulty)
        {
            _maxNoise = (int)(_simulatorRatingController.RatingPerLevel * _simulatorRatingController.AiRatingNoise / 100);
            InitializeReferenceTime(qualificationResult);
            InitializeReferenceRating(difficulty);

            Dictionary<string, DriversRating> ratings = new Dictionary<string, DriversRating>();
            int lastRating = 0;
            foreach (Driver driver in qualificationResult.Where(x => x.IsPlayer || x.BestPersonalLap != null))
            {
                if (driver.IsPlayer)
                {
                    ratings[driver.DriverName] = _simulatorRatingController.GetPlayerRating(driver.ClassName);
                    continue;
                }

                DriverWithoutRating aiDriver = _simulatorRatingController.GetAiRating(driver.DriverName, driver.ClassName);
                lastRating = ComputeRating(driver.BestPersonalLap.LapTime);
                ratings[driver.DriverName] = new DriversRating()
                {
                    Deviation = aiDriver.Deviation,
                    Volatility = aiDriver.Volatility,
                    Rating = lastRating
                };
            }

            foreach (Driver driver in qualificationResult.Where(x => !x.IsPlayer && x.BestPersonalLap == null))
            {
                DriverWithoutRating aiDriver = _simulatorRatingController.GetAiRating(driver.DriverName, driver.ClassName);
                ratings[driver.DriverName] = new DriversRating()
                {
                    Deviation = aiDriver.Deviation,
                    Volatility = aiDriver.Volatility,
                    Rating = lastRating
                };
            }

            return ratings;
        }

        private int ComputeRating(TimeSpan driversTime)
        {
            double percentageDifference = 100 - (driversTime.TotalSeconds / _referenceTime.TotalSeconds) * 100;
            double levelsOfDifference = percentageDifference / _simulatorRatingController.AiTimeDifferencePerLevel;
            int rating = AddNoise((int)(_referenceRating + levelsOfDifference * _simulatorRatingController.RatingPerLevel));
            return rating;
        }

        private int AddNoise(int rating)
        {
            return rating  + _random.Next(_maxNoise) - _maxNoise / 2;
        }

        private void InitializeReferenceRating(int difficulty)
        {
            _referenceRating = AddNoise(_simulatorRatingController.GetRatingForDifficulty(difficulty));
        }

        private void InitializeReferenceTime(List<Driver> drivers)
        {
            List<Driver> driversWithTime = drivers.Where(x => x.BestPersonalLap != null && x.IsPlayer == false).ToList();
            _referenceTime = driversWithTime.Count < 2 ? driversWithTime[0].BestPersonalLap.LapTime : driversWithTime[driversWithTime.Count / 2].BestPersonalLap.LapTime;
        }
    }
}