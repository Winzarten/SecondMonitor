namespace SecondMonitor.Rating.Application.Controller
{
    using DataModel.Snapshot;
    using DataModel.Summary;
    using SecondMonitor.ViewModels.Controllers;
    using ViewModels;

    public interface IRatingApplicationController : IController
    {
        IRatingApplicationViewModel RatingApplicationViewModel { get; }

        void NotifySessionCompletion(SessionSummary sessionSummary);
        void NotifyDataLoaded(SimulatorDataSet simulatorDataSet);
    }
}