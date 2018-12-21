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

        public void StartController()
        {
            StartChildControllers();
        }

        public void StopController()
        {
            _mainWindowController.StopController();
        }

        public async Task OpenFromRepository(string sessionIdentifier)
        {
            await _mainWindowController.LoadTelemetrySession(sessionIdentifier);
        }

        private void StartChildControllers()
        {
            _mainWindowController = TaKernel.Instance.Get<IMainWindowController>();
            _mainWindowController.MainWindow = _mainWindow;
            _mainWindowController.StartController();
        }
    }
}