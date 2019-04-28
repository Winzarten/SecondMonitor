namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using DataModel.BasicProperties;
    using DataModel.Summary;

    public class PracticeState : AbstractSessionTypeState
    {
        public override SessionKind SessionKind { get; protected set; } = SessionKind.OtherSession;

        public override SessionPhaseKind SessionPhaseKind { get; protected set; }
        public override bool CanUserSelectClass => false;

        public override bool DoSessionCompletion(SessionSummary sessionSummary)
        {
            return false;
        }

        protected override SessionType SessionType => SessionType.Practice;
    }
}