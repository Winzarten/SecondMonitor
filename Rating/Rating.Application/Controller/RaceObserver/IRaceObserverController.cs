namespace SecondMonitor.Rating.Application.Controller.RaceObserver
{
    using System.Threading.Tasks;
    using DataModel.Snapshot;
    using DataModel.Summary;
    using RatingProvider;
    using SecondMonitor.ViewModels.Controllers;
    using ViewModels;

    public interface IRaceObserverController : IController, IRatingProvider
    {
        IRatingApplicationViewModel RatingApplicationViewModel { get; set; }

        void Reset();

        Task NotifySessionCompletion(SessionSummary sessionSummary);
        Task NotifyDataLoaded(SimulatorDataSet simulatorDataSet);
    }
}