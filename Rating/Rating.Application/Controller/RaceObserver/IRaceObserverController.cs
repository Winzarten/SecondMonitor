namespace SecondMonitor.Rating.Application.Controller.RaceObserver
{
    using System.Threading.Tasks;
    using DataModel.Snapshot;
    using DataModel.Summary;
    using SecondMonitor.ViewModels.Controllers;
    using ViewModels;

    public interface IRaceObserverController : IController
    {
        IRatingApplicationViewModel RatingApplicationViewModel { get; set; }

        Task NotifySessionCompletion(SessionSummary sessionSummary);
        Task NotifyDataLoaded(SimulatorDataSet simulatorDataSet);
    }
}