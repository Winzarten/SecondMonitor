namespace SecondMonitor.ViewModels.Controllers
{
    using System.Threading.Tasks;

    public interface IController
    {
        Task StartControllerAsync();

        Task StopControllerAsync();
    }
}