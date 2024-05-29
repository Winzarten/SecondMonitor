namespace SecondMonitor.Rating.Application.Controller.RaceObserver
{
    using SimulatorRating;
    using States;

    public interface IRaceStateFactory
    {
        IRaceState CreateInitialState(ISimulatorRatingController simulatorRatingController);
    }
}