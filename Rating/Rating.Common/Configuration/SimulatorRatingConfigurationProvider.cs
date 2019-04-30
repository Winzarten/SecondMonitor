namespace SecondMonitor.Rating.Common.Configuration
{
    using System.Linq;

    public class SimulatorRatingConfigurationProvider : ISimulatorRatingConfigurationProvider
    {
        private readonly SimulatorsRatingConfiguration _simulatorsRatingConfiguration;

        public SimulatorRatingConfigurationProvider()
        {
            _simulatorsRatingConfiguration = new SimulatorsRatingConfiguration()
            {
                SimulatorRatingConfigurations = new[]
                {
                    new SimulatorRatingConfiguration()
                    {
                        SimulatorName = "R3E",
                        MinimumAiLevel = 80,
                        MaximumAiLevel = 120,
                        RatingPerLevel = 300,
                        DefaultPlayerRating = 1500,
                        DefaultPlayerDeviation = 350,
                        DefaultPlayerVolatility = 0.06,
                        MinimumRating = 100,
                        AiTimeDifferencePerLevel = 1,
                        AiRatingNoise = 10,
                    }
                }
            };
        }

        public SimulatorRatingConfiguration GetRatingConfiguration(string simulatorName)
        {
            return _simulatorsRatingConfiguration.SimulatorRatingConfigurations.First(x => x.SimulatorName == simulatorName);
        }
    }
}