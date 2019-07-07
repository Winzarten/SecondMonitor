namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using Common.DataModel.Player;
    using Context;
    using DataModel.Snapshot;
    using DataModel.Summary;

    public interface IRaceState
    {
        SessionKind SessionKind { get; }
        SessionPhaseKind SessionPhaseKind { get; }
        IRaceState NextState { get; }
        string SessionDescription { get; }
        SharedContext SharedContext { get; }
        bool ShowRatingChange { get; }

        bool CanUserSelectClass { get; }
        bool DoSessionCompletion(SessionSummary sessionSummary);
        bool DoDataLoaded(SimulatorDataSet simulatorDataSet);
        bool TryGetDriverRating(string driverName, out DriversRating driversRating);
    }
}