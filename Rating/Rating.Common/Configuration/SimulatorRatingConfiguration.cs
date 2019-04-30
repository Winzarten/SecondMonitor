namespace SecondMonitor.Rating.Common.Configuration
{
    public class SimulatorRatingConfiguration
    {

        public string SimulatorName { get; set; }
        public int MinimumAiLevel { get; set; }
        public int MaximumAiLevel { get; set; }

        public int MinimumRating { get; set; }
        public int RatingPerLevel { get; set; }
        public double AiTimeDifferencePerLevel { get; set; }

        public int DefaultPlayerRating { get; set; }
        public int DefaultPlayerDeviation { get; set; }
        public double DefaultPlayerVolatility { get; set; }

        public double AiRatingNoise { get; set; }
    }
}