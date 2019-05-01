namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using System.Linq;
    using Common.DataModel.Player;
    using Context;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;
    using DataModel.Summary;
    using RatingProvider.FieldRatingProvider;
    using SimulatorRating.RatingUpdater;

    public class RaceState : AbstractSessionTypeState
    {
        private readonly IQualificationResultRatingProvider _qualificationResultRatingProvider;
        private readonly IRatingUpdater _ratingUpdater;

        public RaceState(IQualificationResultRatingProvider qualificationResultRatingProvider, IRatingUpdater ratingUpdater, SharedContext sharedContext) : base(sharedContext)
        {
            _qualificationResultRatingProvider = qualificationResultRatingProvider;
            _ratingUpdater = ratingUpdater;
        }

        public override SessionKind SessionKind { get; protected set; } = SessionKind.RaceWithoutQualification;
        public override SessionPhaseKind SessionPhaseKind { get; protected set; }
        public override bool CanUserSelectClass => false;
        public override bool ShowRatingChange => false;

        protected override void Initialize(SimulatorDataSet simulatorDataSet)
        {
            int difficultyToUse = SharedContext.QualificationContext?.QualificationDifficulty ?? SharedContext.UserSelectedDifficulty;
            SharedContext.RaceContext = new RaceContext()
            {
                UsedDifficulty = difficultyToUse,
            };
            SessionDescription = difficultyToUse.ToString();
            DriverInfo[] eligibleDrivers = FilterEligibleDrivers(simulatorDataSet);
            if (CanUseQualification(eligibleDrivers) && SharedContext.QualificationContext != null)
            {
                SessionKind = SessionKind.RaceWithQualification;
                SharedContext.RaceContext.FieldRating = _qualificationResultRatingProvider.CreateFieldRatingFromQualificationResult(SharedContext.QualificationContext.LastQualificationResult, difficultyToUse);
            }
            else
            {
                SharedContext.RaceContext.FieldRating = _qualificationResultRatingProvider.CreateFieldRating(eligibleDrivers, difficultyToUse);
            }
        }

        private DriverInfo[] FilterEligibleDrivers(SimulatorDataSet simulatorDataSetDataSet)
        {
             var eligibleDrivers = simulatorDataSetDataSet.SessionInfo.IsMultiClass ? simulatorDataSetDataSet.DriversInfo.Where(x => x.CarClassId == simulatorDataSetDataSet.PlayerInfo.CarClassId) : simulatorDataSetDataSet.DriversInfo;
             return eligibleDrivers.ToArray();
        }

        public override bool DoDataLoaded(SimulatorDataSet simulatorDataSet)
        {
            if (simulatorDataSet.SessionInfo.SessionPhase == SessionPhase.Countdown)
            {
                SessionPhaseKind = SessionPhaseKind.NotStarted;
            }
            else if (simulatorDataSet.SessionInfo.LeaderCurrentLap < 1)
            {
                SessionPhaseKind = SessionPhaseKind.FreeRestartPeriod;
            }
            else
            {
                SessionPhaseKind = SessionPhaseKind.InProgress;
            }

            return base.DoDataLoaded(simulatorDataSet);
        }

        public override bool DoSessionCompletion(SessionSummary sessionSummary)
        {
            Driver player = sessionSummary.Drivers.FirstOrDefault(x => x.IsPlayer);
            if (player == null)
            {
                return false;
            }

            if (player.FinishStatus == DriverFinishStatus.Finished)
            {
                ComputeRatingFromResults(sessionSummary);
                SharedContext.QualificationContext = null;
                return false;
            }

            if (SessionPhaseKind == SessionPhaseKind.InProgress)
            {
                ComputeRatingAsLast(sessionSummary);
                SharedContext.QualificationContext = null;
            }
            return false;
        }

        private void ComputeRatingAsLast(SessionSummary sessionSummary)
        {
            Driver player = sessionSummary.Drivers.FirstOrDefault(x => x.IsPlayer);
            if (player == null)
            {
                return;
            }
            _ratingUpdater.UpdateRatingsAsLoss(SharedContext.RaceContext.FieldRating, SharedContext.SimulatorRating, player);
        }

        private void ComputeRatingFromResults(SessionSummary sessionSummary)
        {
            _ratingUpdater.UpdateRatingsByResults(SharedContext.RaceContext.FieldRating, SharedContext.SimulatorRating, sessionSummary);
        }

        private bool CanUseQualification(DriverInfo[] eligibleDrivers)
        {
            if (SharedContext.QualificationContext?.LastQualificationResult == null)
            {
                return false;
            }


            return !SharedContext.QualificationContext.LastQualificationResult.Select(y => y.DriverName).Except(eligibleDrivers.Select(x => x.DriverName)).Any();
        }

        protected override SessionType SessionType => SessionType.Race;

        public override bool TryGetDriverRating(string driverName, out DriversRating driversRating)
        {
            if (SharedContext?.RaceContext?.FieldRating != null)
            {
                return SharedContext.RaceContext.FieldRating.TryGetValue(driverName, out driversRating);
            }


            driversRating = new DriversRating();
            return false;

        }
    }
}