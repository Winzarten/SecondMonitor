namespace SecondMonitor.Rating.Common.Configuration
{
    public interface ISimulatorRatingConfigurationProvider
    {
        SimulatorRatingConfiguration GetRatingConfiguration(string simulatorName);
    }
}