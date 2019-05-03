namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using System;
    using Common.DataModel.Player;
    using Context;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Summary;
    using RatingProvider.FieldRatingProvider;
    using SimulatorRating.RatingUpdater;

    public abstract class AbstractSessionTypeState : IRaceState
    {
        protected AbstractSessionTypeState(SharedContext sharedContext)
        {
            SharedContext = sharedContext;
            SessionDescription = string.Empty;
        }

        public abstract SessionKind SessionKind { get; protected set; }
        public abstract SessionPhaseKind SessionPhaseKind { get; protected set; }
        public IRaceState NextState { get; protected set; }
        public string SessionDescription { get; protected set; }

        public abstract bool ShowRatingChange { get; }

        public abstract bool CanUserSelectClass { get; }
        protected abstract SessionType SessionType { get; }
        public abstract bool DoSessionCompletion(SessionSummary sessionSummary);

        public SharedContext SharedContext { get; }

        protected abstract void Initialize(SimulatorDataSet simulatorDataSet);
        protected bool IsStateInitialized { get; set; }

        public virtual bool DoDataLoaded(SimulatorDataSet simulatorDataSet)
        {
            if (!IsStateInitialized && simulatorDataSet.SessionInfo.SessionPhase != SessionPhase.Unavailable)
            {
                Initialize(simulatorDataSet);
                IsStateInitialized = true;
            }

            if (simulatorDataSet.SessionInfo.SessionType != SessionType.Race)
            {
                SessionPhaseKind = simulatorDataSet.SessionInfo.SessionPhase == SessionPhase.Countdown ? SessionPhaseKind.NotStarted : SessionPhaseKind.InProgress;
            }

            if (simulatorDataSet.SessionInfo.SessionType == SessionType)
            {
                return false;
            }

            switch (simulatorDataSet.SessionInfo.SessionType)
            {
                case SessionType.Na:
                    NextState = new IdleState(SharedContext);
                    break;
                case SessionType.WarmUp:
                    NextState = new WarmupState(SharedContext);
                    break;
                case SessionType.Practice:
                    NextState = new PracticeState(SharedContext);
                    break;
                case SessionType.Qualification:
                    NextState = new QualificationState(SharedContext);
                    break;
                case SessionType.Race:
                    NextState = new RaceState(new QualificationResultRatingProvider(SharedContext.SimulatorRatingController), new RatingUpdater(SharedContext.SimulatorRatingController),   SharedContext);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        public virtual bool TryGetDriverRating(string driverName, out DriversRating driversRating)
        {
            driversRating = new DriversRating();
            return false;
        }
    }
}