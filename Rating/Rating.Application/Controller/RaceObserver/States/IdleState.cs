namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Summary;

    public class IdleState : AbstractSessionTypeState
    {
        public override SessionKind SessionKind { get; protected set; } = SessionKind.Idle;
        public override SessionPhaseKind SessionPhaseKind { get; protected set; } = SessionPhaseKind.None;
        public override bool CanUserSelectClass => true;

        public override bool DoDataLoaded(SimulatorDataSet simulatorDataSet)
        {
            return simulatorDataSet.SessionInfo.SessionType != SessionType && base.DoDataLoaded(simulatorDataSet);
        }

        public override bool DoSessionCompletion(SessionSummary sessionSummary)
        {
            return false;
        }

        protected override SessionType SessionType => SessionType.Na;
    }
}