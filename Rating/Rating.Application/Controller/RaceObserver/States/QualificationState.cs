namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Documents;
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
        public override bool ShowRatingChange => false;

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

            List<Driver> eligibleDrivers = FilterNotEligibleDriversAndOrder(sessionSummary);
            if (eligibleDrivers == null)
            {
                SharedContext.QualificationContext = null;
                return false;
            }

            SharedContext.QualificationContext.LastQualificationResult = eligibleDrivers;
            return false;
        }

        private bool IsSessionResultAcceptable(SessionSummary sessionSummary)
        {
            return sessionSummary.Drivers.Count(x => x.BestPersonalLap != null) > sessionSummary.Drivers.Count / 2;
        }

        private List<Driver> FilterNotEligibleDriversAndOrder(SessionSummary sessionSummary)
        {
            if (!sessionSummary.IsMultiClass)
            {
                return sessionSummary.Drivers.OrderBy(x => x.FinishingPosition).ToList();
            }

            Driver player = sessionSummary.Drivers.FirstOrDefault(x => x.IsPlayer);
            return player == null ? null : sessionSummary.Drivers.Where(x => x.ClassId == player.ClassId).OrderBy(x => x.FinishingPosition).ToList();
        }

    }
}