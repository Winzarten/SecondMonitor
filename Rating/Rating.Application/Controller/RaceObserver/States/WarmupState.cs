namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States
{
    using DataModel.BasicProperties;

    public class WarmupState : PracticeState
    {
        protected override SessionType SessionType => SessionType.WarmUp;
    }
}