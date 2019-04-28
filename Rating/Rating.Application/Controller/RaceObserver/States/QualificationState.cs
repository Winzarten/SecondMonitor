namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using DataModel.BasicProperties;
    using DataModel.Summary;

    public class QualificationState : AbstractSessionTypeState
    {
        public override SessionKind SessionKind { get; protected set; } = SessionKind.Qualification;
        public override SessionPhaseKind SessionPhaseKind { get; protected set; }
        public override bool CanUserSelectClass => false;
        protected override SessionType SessionType => SessionType.Qualification;

        public override bool DoSessionCompletion(SessionSummary sessionSummary)
        {
            return false;
        }


    }
}