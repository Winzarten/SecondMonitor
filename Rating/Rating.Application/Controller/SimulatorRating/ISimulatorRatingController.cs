namespace SecondMonitor.Rating.Application.Controller.SimulatorRating
{
    using System.Collections.Generic;
    using Common.DataModel.Player;
    using SecondMonitor.ViewModels.Controllers;

    public interface ISimulatorRatingController : IController
    {
        int MinimumAiDifficulty { get; }
        int MaximumAiDifficulty { get; }

        DriversRating GetPlayerOverallRating();
        DriverWithoutRating GetAiRating(string aiDriverName, string className);
        DriversRating GetPlayerRating(string className);
        int GetSuggestedDifficulty(string className);
        IReadOnlyCollection<string> GetAllKnowClassNames();

    }
}