namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.OpenWindow
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using WindowsControls.WPF.Commands;
    using Factory;
    using TelemetryLoad;
    using TelemetryManagement.DTO;
    using ViewModels;
    using ViewModels.OpenWindow;

    public class OpenWindowController : IOpenWindowController
    {
        private readonly IMainWindowViewModel _mainWindowViewModel;
        private readonly ITelemetryLoadController _telemetryLoadController;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IOpenWindowViewModel _openWindowViewModel;

        public OpenWindowController(IMainWindowViewModel mainWindowViewModel, ITelemetryLoadController telemetryLoadController, IViewModelFactory viewModelFactory)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _telemetryLoadController = telemetryLoadController;
            _viewModelFactory = viewModelFactory;
            _openWindowViewModel = viewModelFactory.Create<IOpenWindowViewModel>();
            _openWindowViewModel.RefreshRecentCommand = new AsyncCommand(RefreshAvailableSession);
            _mainWindowViewModel.LapSelectionViewModel.OpenWindowViewModel = _openWindowViewModel;
        }

        public Task StartControllerAsync()
        {
            return Task.CompletedTask;
        }

        public Task StopControllerAsync()
        {
            return Task.CompletedTask;
        }

        private async Task RefreshAvailableSession()
        {
            IReadOnlyCollection<SessionInfoDto> sessionInfos = await _telemetryLoadController.GetAllRecentSessionInfoAsync();
        }
    }
}