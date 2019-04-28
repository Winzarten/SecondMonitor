namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Summary;

    public abstract class AbstractSessionTypeState : IRaceState
    {
        public abstract SessionKind SessionKind { get; protected set; }
        public abstract SessionPhaseKind SessionPhaseKind { get; protected set; }
        public IRaceState NextState { get; protected set; }
        public abstract bool CanUserSelectClass { get; }
        protected abstract SessionType SessionType { get; }
        public abstract bool DoSessionCompletion(SessionSummary sessionSummary);

        public virtual bool DoDataLoaded(SimulatorDataSet simulatorDataSet)
        {
            SessionPhaseKind = simulatorDataSet.SessionInfo.SessionPhase == SessionPhase.Countdown ? SessionPhaseKind.NotStarted : SessionPhaseKind.InProgress;
            if (simulatorDataSet.SessionInfo.SessionType == SessionType)
            {
                return false;
            }

            switch (simulatorDataSet.SessionInfo.SessionType)
            {
                case SessionType.Na:
                    NextState = new IdleState();
                    break;
                case SessionType.WarmUp:
                    NextState = new WarmupState();
                    break;
                case SessionType.Practice:
                    NextState = new PracticeState();
                    break;
                case SessionType.Qualification:
                    NextState = new QualificationState();
                    break;
                case SessionType.Race:
                    NextState = new RaceState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }
    }
}