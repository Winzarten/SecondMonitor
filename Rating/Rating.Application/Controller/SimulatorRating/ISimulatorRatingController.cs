namespace SecondMonitor.Rating.Application.Controller.SimulatorRating
{
    using System;
    using System.Collections.Generic;
    using Common.DataModel;
    using Common.DataModel.Player;
    using RatingProvider;
    using SecondMonitor.ViewModels.Controllers;

    public interface ISimulatorRatingController : IController
    {
        event EventHandler<RatingChangeArgs> ClassRatingChanged;
        event EventHandler<RatingChangeArgs> SimulatorRatingChanged;

        int MinimumAiDifficulty { get; }
        int MaximumAiDifficulty { get; }
        double AiTimeDifferencePerLevel { get; }
        double AiRatingNoise { get; }
        int RatingPerLevel { get; }
        int QuickRaceAiRatingForPlace { get; }
        string SimulatorName { get; }

        DriversRating GetPlayerOverallRating();
        DriverWithoutRating GetAiRating(string aiDriverName);
        DriversRating GetPlayerRating(string className);
        void SetSelectedDifficulty(int difficulty, bool wasUserSelected, string className);
        void UpdateRating(DriversRating newClassRating, DriversRating newSimRating, string className, string trackName);
        string GetRaceSuggestion();


        int GetSuggestedDifficulty(string className);
        DifficultySettings GetDifficultySettings(string className);
        int GetRatingForDifficulty(int aiDifficulty);
        IReadOnlyCollection<string> GetAllKnowClassNames();

    }
}