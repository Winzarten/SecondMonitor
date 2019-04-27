namespace SecondMonitor.Rating.Application.ViewModels
{
    using Common.DataModel.Player;
    using Rating;
    using SecondMonitor.ViewModels;

    public interface IRatingApplicationViewModel : IViewModel
    {
        IRatingViewModel SimulatorRating { get; }
        IRatingViewModel ClassRating { get; }

        void ApplySimulatorRating(DriversRating driversRating);
        void ApplyClassRating(DriversRating driversRating);

    }
}