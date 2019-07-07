namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using Context;
    using DataModel.BasicProperties;

    public class WarmupState : PracticeState
    {
        protected override SessionType SessionType => SessionType.WarmUp;

        public WarmupState(SharedContext sharedContext) : base(sharedContext)
        {
        }
    }
}