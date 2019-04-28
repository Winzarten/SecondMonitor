namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using System.Threading.Tasks;
    using DataModel.Snapshot;
    using DataModel.Summary;

    public class IdleState : IRaceState
    {
        public SessionKind SessionKind => SessionKind.Idle;
        public SessionPhaseKind SessionPhaseKind => SessionPhaseKind.None;
        public Task<bool> DoSessionCompletion(SessionSummary sessionSummary)
        {
            return Task.FromResult(false);
        }

        public Task<bool> DoDataLoaded(SimulatorDataSet simulatorDataSet)
        {
            return Task.FromResult(false);
        }

        public IRaceState GetNextState()
        {
            return null;
        }
    }
}