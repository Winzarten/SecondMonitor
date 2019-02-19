namespace SecondMonitor.Remote.Application.Controllers
{
    using DataModel.Snapshot;
    using SecondMonitor.ViewModels.Controllers;

    public interface IBroadCastServerController : IController
    {
        void SendSessionStartedPackage(SimulatorDataSet simulatorDataSet);
        void SendRegularDataPackage(SimulatorDataSet simulatorDataSet);
    }
}