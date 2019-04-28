namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Summary;

    public class RaceState : AbstractSessionTypeState
    {
        public override SessionKind SessionKind { get; protected set; } = SessionKind.RaceWithoutQualification;
        public override SessionPhaseKind SessionPhaseKind { get; protected set; }
        public override bool CanUserSelectClass => false;
        public override bool DoSessionCompletion(SessionSummary sessionSummary)
        {
            return false;
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

            return simulatorDataSet.SessionInfo.SessionType != SessionType && base.DoDataLoaded(simulatorDataSet);
        }

        protected override SessionType SessionType => SessionType.Race;
    }
}