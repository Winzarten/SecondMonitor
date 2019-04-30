namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using System.Linq;
    using Common.DataModel.Player;
    using Context;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Summary;
    using RatingProvider.FieldRatingProvider;

    public class RaceState : AbstractSessionTypeState
    {
        private readonly IQualificationResultRatingProvider _qualificationResultRatingProvider;

        public RaceState(IQualificationResultRatingProvider qualificationResultRatingProvider, SharedContext sharedContext) : base(sharedContext)
        {
            _qualificationResultRatingProvider = qualificationResultRatingProvider;
        }

        public override SessionKind SessionKind { get; protected set; } = SessionKind.RaceWithoutQualification;
        public override SessionPhaseKind SessionPhaseKind { get; protected set; }
        public override bool CanUserSelectClass => false;
        public override bool DoSessionCompletion(SessionSummary sessionSummary)
        {
            return false;
        }

        protected override void Initialize(SimulatorDataSet simulatorDataSet)
        {
            int difficultyToUse = SharedContext.QualificationContext?.QualificationDifficulty ?? SharedContext.UserSelectedDifficulty;
            SharedContext.RaceContext = new RaceContext()
            {
                UsedDifficulty = difficultyToUse,
            };
            SessionDescription = difficultyToUse.ToString();

            if (CanUseQualification(simulatorDataSet))
            {
                SessionKind = SessionKind.RaceWithQualification;
                SharedContext.RaceContext.FieldRating = _qualificationResultRatingProvider.CreateFieldRating(SharedContext.QualificationContext.LastQualificationResult, difficultyToUse);
                return;
            }
        }

        public override bool DoDataLoaded(SimulatorDataSet simulatorDataSet)
        {
            if (simulatorDataSet.SessionInfo.SessionPhase == SessionPhase.Countdown)
            {
                SessionPhaseKind = SessionPhaseKind.NotStarted;
            }
            else if (simulatorDataSet.SessionInfo.LeaderCurrentLap < 2)
            {
                SessionPhaseKind = SessionPhaseKind.FreeRestartPeriod;
            }
            else
            {
                SessionPhaseKind = SessionPhaseKind.InProgress;
            }

            return base.DoDataLoaded(simulatorDataSet);
        }

        private bool CanUseQualification(SimulatorDataSet simulatorDataSetDataSet)
        {
            if (SharedContext.QualificationContext?.LastQualificationResult == null)
            {
                return false;
            }

            return !SharedContext.QualificationContext.LastQualificationResult.Select(y => y.DriverName).Except(simulatorDataSetDataSet.DriversInfo.Select(x => x.DriverName)).Any();
        }

        protected override SessionType SessionType => SessionType.Race;

        public override bool TryGetDriverRating(string driverName, out DriversRating driversRating)
        {
            if (SharedContext?.RaceContext?.FieldRating != null)
            {
                return SharedContext.RaceContext.FieldRating.TryGetValue(driverName, out driversRating);
            }


            driversRating = null;
            return false;

        }
    }
}