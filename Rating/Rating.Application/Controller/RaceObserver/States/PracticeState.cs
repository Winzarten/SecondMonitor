namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using Context;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Summary;

    public class PracticeState : AbstractSessionTypeState
    {
        public PracticeState(SharedContext sharedContext) : base(sharedContext)
        {
            SharedContext.QualificationContext = null;
            SharedContext.RaceContext = null;
        }

        public override SessionKind SessionKind { get; protected set; } = SessionKind.OtherSession;

        public override SessionPhaseKind SessionPhaseKind { get; protected set; }
        public override bool CanUserSelectClass => false;

        public override bool ShowRatingChange => true;

        public override bool DoSessionCompletion(SessionSummary sessionSummary)
        {
            return false;
        }

        protected override void Initialize(SimulatorDataSet simulatorDataSet)
        {
        }


        protected override SessionType SessionType => SessionType.Practice;
    }
}