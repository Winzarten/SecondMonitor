namespace SecondMonitor.Rating.Application.Controller.SimulatorRating
{
    using Common.DataModel.Player;
    using SecondMonitor.ViewModels.Controllers;

    public interface ISimulatorRatingController : IController
    {
        DriversRating GetPlayerOverallRating();
        DriverWithoutRating GetAiRating(string aiDriverName, string className);
        DriversRating GetPlayerRating(string className);
    }
}