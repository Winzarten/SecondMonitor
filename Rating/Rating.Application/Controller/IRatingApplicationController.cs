namespace SecondMonitor.Rating.Application.Controller
{
    using System.Threading.Tasks;
    using DataModel.Snapshot;
    using DataModel.Summary;
    using RatingProvider;
    using SecondMonitor.ViewModels.Controllers;
    using ViewModels;

    public interface IRatingApplicationController : IController
    {
        IRatingApplicationViewModel RatingApplicationViewModel { get; }
        IRatingProvider RatingProvider { get; }


        Task NotifySessionCompletion(SessionSummary sessionSummary);
        Task NotifyDataLoaded(SimulatorDataSet simulatorDataSet);
    }
}