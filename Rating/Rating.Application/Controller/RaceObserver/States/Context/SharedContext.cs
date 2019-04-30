namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States.Context
{
    using SimulatorRating;

    public class SharedContext
    {
        public int UserSelectedDifficulty { get; set; }
        public QualificationContext QualificationContext { get; set; }
        public RaceContext RaceContext { get; set; }
        public ISimulatorRatingController SimulatorRatingController { get; set; }
    }
}