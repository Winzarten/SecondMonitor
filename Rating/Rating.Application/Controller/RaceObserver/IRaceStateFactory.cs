namespace SecondMonitor.Rating.Application.Controller.RaceObserver
{
    using States;

    public interface IRaceStateFactory
    {
        IRaceState CreateInitialState(string simulatorName);
    }
}