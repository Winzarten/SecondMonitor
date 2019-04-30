namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using Context;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Summary;

    public class IdleState : AbstractSessionTypeState
    {
        public IdleState(SharedContext sharedContext) : base(sharedContext)
        {

        }

        public override SessionKind SessionKind { get; protected set; } = SessionKind.Idle;
        public override SessionPhaseKind SessionPhaseKind { get; protected set; } = SessionPhaseKind.None;
        public override bool CanUserSelectClass => true;

        protected override void Initialize(SimulatorDataSet simulatorDataSet)
        {

        }

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