namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.OpenWindow
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts.Commands;
    using SecondMonitor.ViewModels.Factory;
    using Synchronization;
    using TelemetryLoad;
    using TelemetryManagement.DTO;
    using ViewModels;
    using ViewModels.OpenWindow;

    public class OpenWindowController : IOpenWindowController
    {
        private readonly IMainWindowViewModel _mainWindowViewModel;
        private readonly ITelemetryLoadController _telemetryLoadController;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;
        private readonly IOpenWindowViewModel _openWindowViewModel;
        private readonly IOpenWindowViewModel _addWindowViewModel;
        private SessionInfoDto _lastOpenedSession;
        private readonly List<SessionInfoDto> _loadedSessions;

        public OpenWindowController(IMainWindowViewModel mainWindowViewModel, ITelemetryLoadController telemetryLoadController, IViewModelFactory viewModelFactory, ITelemetryViewsSynchronization telemetryViewsSynchronization)
        {
            _loadedSessions = new List<SessionInfoDto>();
            _mainWindowViewModel = mainWindowViewModel;
            _telemetryLoadController = telemetryLoadController;
            _viewModelFactory = viewModelFactory;
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _openWindowViewModel = viewModelFactory.Create<IOpenWindowViewModel>();
            _addWindowViewModel = viewModelFactory.Create<IOpenWindowViewModel>();
            _mainWindowViewModel.LapSelectionViewModel.OpenWindowViewModel = _openWindowViewModel;
            _mainWindowViewModel.LapSelectionViewModel.AddWindowViewModel = _addWindowViewModel;
            BindCommands();
        }

        public Task StartControllerAsync()
        {
            _telemetryViewsSynchronization.NewSessionLoaded+= TelemetryViewsSynchronizationOnNewSessionLoaded;
            _telemetryViewsSynchronization.SessionAdded += TelemetryViewsSynchronizationOnSessionAdded;
            return Task.CompletedTask;
        }

        public Task StopControllerAsync()
        {
            _telemetryViewsSynchronization.NewSessionLoaded -= TelemetryViewsSynchronizationOnNewSessionLoaded;
            _telemetryViewsSynchronization.SessionAdded -= TelemetryViewsSynchronizationOnSessionAdded;
            return Task.CompletedTask;
        }

        private async Task RefreshAllAvailableSession(IOpenWindowViewModel openWindowViewModel)
        {
            openWindowViewModel.IsBusy = true;
            IReadOnlyCollection<SessionInfoDto> sessionInfos = await _telemetryLoadController.GetAllRecentSessionInfoAsync();
            openWindowViewModel.RecentSessionsInfos = sessionInfos.OrderByDescending(x => x.SessionRunDateTime).ToList().AsReadOnly();
            openWindowViewModel.IsBusy = false;
        }

        private async Task RefreshAvailableSessionThatMatchTrack(IOpenWindowViewModel openWindowViewModel)
        {
            openWindowViewModel.IsBusy = true;
            IReadOnlyCollection<SessionInfoDto> sessionInfos = await _telemetryLoadController.GetAllRecentSessionInfoAsync();
            if (_lastOpenedSession != null)
            {
                sessionInfos = sessionInfos.Where(x => x.TrackName == _lastOpenedSession.TrackName && x.LayoutName == _lastOpenedSession.LayoutName && x.Simulator == _lastOpenedSession.Simulator && _loadedSessions.FirstOrDefault(y => y.Id == x.Id) == null).ToList();
            }
            openWindowViewModel.RecentSessionsInfos = sessionInfos.OrderByDescending(x => x.SessionRunDateTime).ToList().AsReadOnly();
            openWindowViewModel.IsBusy = false;
        }

        private void CancelAndCloseOpenWindow(IOpenWindowViewModel openWindowViewModel)
        {
            openWindowViewModel.IsOpenWindowVisible = false;
        }

        private void TelemetryViewsSynchronizationOnSessionAdded(object sender, TelemetrySessionArgs e)
        {
            _loadedSessions.Add(e.SessionInfoDto);
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

        private async Task AddSelectedRecentSession()
        {
            if (_addWindowViewModel.SelectedRecentSessionInfoDto == null)
            {
                return;
            }
            _addWindowViewModel.IsOpenWindowVisible = false;
            await _telemetryLoadController.AddRecentSessionAsync(_addWindowViewModel.SelectedRecentSessionInfoDto);
        }

        private void TelemetryViewsSynchronizationOnNewSessionLoaded(object sender, TelemetrySessionArgs e)
        {
            _loadedSessions.Clear();
            _loadedSessions.Add(e.SessionInfoDto);
            _lastOpenedSession = e.SessionInfoDto;
        }


        private void BindCommands()
        {
            _openWindowViewModel.RefreshRecentCommand = new AsyncCommand(() => RefreshAllAvailableSession(_openWindowViewModel));
            _openWindowViewModel.CancelAndCloseWindowCommand = new RelayCommand(() => CancelAndCloseOpenWindow(_openWindowViewModel));
            _openWindowViewModel.OpenSelectedRecentSessionCommand = new AsyncCommand(OpenSelectedRecentSession);

            _addWindowViewModel.RefreshRecentCommand = new AsyncCommand(() => RefreshAvailableSessionThatMatchTrack(_addWindowViewModel));
            _addWindowViewModel.CancelAndCloseWindowCommand = new RelayCommand(() => CancelAndCloseOpenWindow(_addWindowViewModel));
            _addWindowViewModel.OpenSelectedRecentSessionCommand = new AsyncCommand(AddSelectedRecentSession);
        }
    }
}