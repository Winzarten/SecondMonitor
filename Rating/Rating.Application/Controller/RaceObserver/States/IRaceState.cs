namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using DataModel.Snapshot;
    using DataModel.Summary;

    public interface IRaceState
    {
        SessionKind SessionKind { get; }
        SessionPhaseKind SessionPhaseKind { get; }
        IRaceState NextState { get; }

        bool CanUserSelectClass { get; }
        bool DoSessionCompletion(SessionSummary sessionSummary);
        bool DoDataLoaded(SimulatorDataSet simulatorDataSet);
    }
}