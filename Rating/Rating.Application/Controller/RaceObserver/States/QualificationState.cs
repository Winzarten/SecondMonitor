namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using System.Linq;
    using Context;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Summary;

    public class QualificationState : AbstractSessionTypeState
    {
        public QualificationState(SharedContext sharedContext) : base(sharedContext)
        {
        }


        public override SessionKind SessionKind { get; protected set; } = SessionKind.Qualification;
        public override SessionPhaseKind SessionPhaseKind { get; protected set; }
        public override bool CanUserSelectClass => false;
        protected override SessionType SessionType => SessionType.Qualification;

        protected override void Initialize(SimulatorDataSet simulatorDataSet)
        {
            SharedContext.QualificationContext = new QualificationContext() { QualificationDifficulty = SharedContext.UserSelectedDifficulty };
            SessionDescription = SharedContext.QualificationContext.QualificationDifficulty.ToString();
        }

        public override bool DoSessionCompletion(SessionSummary sessionSummary)
        {
            if (!IsSessionResultAcceptable(sessionSummary))
            {
                SharedContext.QualificationContext = null;
                return false;
            }

            SharedContext.QualificationContext.LastQualificationResult = sessionSummary.Drivers.OrderBy(x => x.FinishingPosition).ToList();
            return false;
        }

        private bool IsSessionResultAcceptable(SessionSummary sessionSummary)
        {
            return sessionSummary.Drivers.Count(x => x.BestPersonalLap != null) > sessionSummary.Drivers.Count / 2;
        }

    }
}