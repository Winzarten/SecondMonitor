namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers
{
    using System.Threading.Tasks;
    using System.Windows;
    using IOC;
    using MainWindow;
    using SecondMonitor.ViewModels.Controllers;

    public class TelemetryApplicationController : IController
    {
        private readonly Window _mainWindow;

        private IMainWindowController _mainWindowController;

        public TelemetryApplicationController(Window mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public async Task StartControllerAsync()
        {
            await StartChildControllers();
        }

        public async Task StopControllerAsync()
        {
            await _mainWindowController.StopControllerAsync();
        }

        public async Task OpenFromRepository(string sessionIdentifier)
        {
            await _mainWindowController.LoadTelemetrySession(sessionIdentifier);
        }

        public async Task OpenLastSessionFromRepository()
        {
            await _mainWindowController.LoadLastTelemetrySession();
        }

        private async Task StartChildControllers()
        {
            _mainWindowController = TaKernel.Instance.Get<IMainWindowController>();
            _mainWindowController.MainWindow = _mainWindow;
            await _mainWindowController.StartControllerAsync();
        }
    }
}