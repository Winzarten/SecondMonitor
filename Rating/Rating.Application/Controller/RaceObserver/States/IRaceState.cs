namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using System.Threading.Tasks;
    using DataModel.Snapshot;
    using DataModel.Summary;

    public interface IRaceState
    {
        SessionKind SessionKind { get; }
        SessionPhaseKind SessionPhaseKind { get; }

        Task<bool> DoSessionCompletion(SessionSummary sessionSummary);
        Task<bool> DoDataLoaded(SimulatorDataSet simulatorDataSet);
        IRaceState GetNextState();
    }
}