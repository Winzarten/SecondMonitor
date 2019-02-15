namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.OpenWindow
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts.Commands;
    using SecondMonitor.ViewModels.Factory;
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
            _mainWindowViewModel.LapSelectionViewModel.OpenWindowViewModel = _openWindowViewModel;
            BindCommands();
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
            _openWindowViewModel.IsBusy = true;
            IReadOnlyCollection<SessionInfoDto> sessionInfos = await _telemetryLoadController.GetAllRecentSessionInfoAsync();
            _openWindowViewModel.RecentSessionsInfos = sessionInfos.OrderByDescending(x => x.SessionRunDateTime).ToList().AsReadOnly();
            _openWindowViewModel.IsBusy = false;
        }

        private void CancelAndCloseOpenWindow()
        {
            _openWindowViewModel.IsOpenWindowVisible = false;
        }

        private async Task OpenSelectedRecentSession()
        {
            if (_openWindowViewModel.SelectedRecentSessionInfoDto == null)
            {
                return;
            }
            _openWindowViewModel.IsOpenWindowVisible = false;
            await _telemetryLoadController.LoadRecentSessionAsync(_openWindowViewModel.SelectedRecentSessionInfoDto);
        }


        private void BindCommands()
        {
            _openWindowViewModel.RefreshRecentCommand = new AsyncCommand(RefreshAvailableSession);
            _openWindowViewModel.CancelAndCloseWindowCommand = new RelayCommand(CancelAndCloseOpenWindow);
            _openWindowViewModel.OpenSelectedRecentSessionCommand = new AsyncCommand(OpenSelectedRecentSession);
        }
    }
}