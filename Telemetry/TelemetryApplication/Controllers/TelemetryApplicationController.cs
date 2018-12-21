namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers
{
    using System.Threading.Tasks;
    using System.Windows;
    using MainWindow;
    using Ninject;
    using ViewModels.Controllers;

    public class TelemetryApplicationController : IController
    {
        private readonly Window _mainWindow;
        private IKernel _kernel;
        private IMainWindowController _mainWindowController;

        public TelemetryApplicationController(Window mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void StartController()
        {
            InitializeKernel();
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
            _mainWindowController = _kernel.Get<IMainWindowController>();
            _mainWindowController.MainWindow = _mainWindow;
            _mainWindowController.StartController();
        }

        private void InitializeKernel()
        {
            _kernel = new StandardKernel(new TelemetryApplicationModule());
        }
    }
}