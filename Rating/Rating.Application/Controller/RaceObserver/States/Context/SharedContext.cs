namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States.Context
{
    using Common.DataModel.Player;
    using SimulatorRating;

    public class SharedContext
    {
        public int UserSelectedDifficulty { get; set; }
        public QualificationContext QualificationContext { get; set; }
        public RaceContext RaceContext { get; set; }
        public ISimulatorRatingController SimulatorRatingController { get; set; }
        public DriversRating SimulatorRating { get; set; }
    }
}