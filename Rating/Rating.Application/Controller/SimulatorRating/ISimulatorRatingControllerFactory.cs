namespace SecondMonitor.Rating.Application.Controller.SimulatorRating
{
    public interface ISimulatorRatingControllerFactory
    {
        bool IsSimulatorSupported(string simulatorName);
        ISimulatorRatingController CreateController(string simulatorName);
    }
}