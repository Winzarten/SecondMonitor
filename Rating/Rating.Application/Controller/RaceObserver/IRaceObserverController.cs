namespace SecondMonitor.Rating.Application.Controller.RaceObserver
{
    using System.Threading.Tasks;
    using DataModel.Snapshot;
    using DataModel.Summary;
    using SecondMonitor.ViewModels.Controllers;

    public interface IRaceObserverController : IController
    {
        Task NotifySessionCompletion(SessionSummary sessionSummary);
        Task NotifyDataLoaded(SimulatorDataSet simulatorDataSet);
    }
}