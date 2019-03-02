namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.OpenWindow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Contracts.Commands;
    using NLog;
    using SecondMonitor.ViewModels.Factory;
    using Synchronization;
    using TelemetryLoad;
    using TelemetryManagement.DTO;
    using ViewModels;
    using ViewModels.OpenWindow;

    public class OpenWindowController : IOpenWindowController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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
            Task<IReadOnlyCollection<SessionInfoDto>> recentSessionsTask = _telemetryLoadController.GetAllRecentSessionInfoAsync();
            Task<IReadOnlyCollection<SessionInfoDto>> archiveSessionsTask = _telemetryLoadController.GetAllArchivedSessionInfoAsync();

            await Task.WhenAll(recentSessionsTask, archiveSessionsTask);

            SetAvailableSessions(openWindowViewModel, recentSessionsTask.Result, archiveSessionsTask.Result, OpenSession);
            openWindowViewModel.IsBusy = false;
        }

        private void SetAvailableSessions(IOpenWindowViewModel openWindowViewModel, IReadOnlyCollection<SessionInfoDto> recentSessionInfos, IReadOnlyCollection<SessionInfoDto> archivedSessionInfoDtos, Func<SessionInfoDto, Task> openByDoubleClickCommand)
        {
            openWindowViewModel.RecentSessionsInfos = recentSessionInfos.OrderByDescending(x => x.SessionRunDateTime).Select(x =>
            {
                IOpenWindowSessionInformationViewModel newViewModel = _viewModelFactory.Create<IOpenWindowSessionInformationViewModel>();
                newViewModel.FromModel(x);
                newViewModel.ArchiveCommand = new AsyncCommand(() => ArchiveSession(openWindowViewModel, x));
                newViewModel.SelectThisSessionCommand = new AsyncCommand(() => openByDoubleClickCommand(x));
                newViewModel.ShowArchiveIcon = true;
                return newViewModel;
            }).ToList();

            openWindowViewModel.ArchiveSessionsInfos = archivedSessionInfoDtos.OrderByDescending(x => x.SessionRunDateTime).Select(x =>
            {
                IOpenWindowSessionInformationViewModel newViewModel = _viewModelFactory.Create<IOpenWindowSessionInformationViewModel>();
                newViewModel.FromModel(x);
                newViewModel.ShowArchiveIcon = false;
                newViewModel.SelectThisSessionCommand = new AsyncCommand(() => openByDoubleClickCommand(x));
                return newViewModel;
            }).ToList();
        }

        private async Task RefreshAvailableSessionThatMatchTrack(IOpenWindowViewModel openWindowViewModel)
        {
            openWindowViewModel.IsBusy = true;
            Task<IReadOnlyCollection<SessionInfoDto>> recentSessionsTask = _telemetryLoadController.GetAllRecentSessionInfoAsync();
            Task<IReadOnlyCollection<SessionInfoDto>> archiveSessionsTask = _telemetryLoadController.GetAllArchivedSessionInfoAsync();

            await Task.WhenAll(recentSessionsTask, archiveSessionsTask);

            IReadOnlyCollection<SessionInfoDto> recentSessionInfos = recentSessionsTask.Result;
            IReadOnlyCollection<SessionInfoDto> archivedSessionsInfo = archiveSessionsTask.Result;
            if (_lastOpenedSession != null)
            {
                recentSessionInfos = recentSessionInfos.Where(x => x.TrackName == _lastOpenedSession.TrackName && x.LayoutName == _lastOpenedSession.LayoutName && x.Simulator == _lastOpenedSession.Simulator && _loadedSessions.FirstOrDefault(y => y.Id == x.Id) == null).ToList();
                archivedSessionsInfo = archivedSessionsInfo.Where(x => x.TrackName == _lastOpenedSession.TrackName && x.LayoutName == _lastOpenedSession.LayoutName && x.Simulator == _lastOpenedSession.Simulator && _loadedSessions.FirstOrDefault(y => y.Id == x.Id) == null).ToList();
            }
            SetAvailableSessions(openWindowViewModel, recentSessionInfos, archivedSessionsInfo, AddSession);
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
            IOpenWindowSessionInformationViewModel selectedViewModel = _openWindowViewModel.SelectedTabIndex == 0 ? _openWindowViewModel.SelectedRecentSessionInfoDto : _openWindowViewModel.SelectedArchiveSessionInfoDto;
            if (selectedViewModel == null)
            {
                return;
            }

            await OpenSession(selectedViewModel.OriginalModel);
        }

        private async Task AddSelectedRecentSession()
        {
            IOpenWindowSessionInformationViewModel selectedViewModel = _addWindowViewModel.SelectedTabIndex == 0 ? _addWindowViewModel.SelectedRecentSessionInfoDto : _addWindowViewModel.SelectedArchiveSessionInfoDto;
            if (selectedViewModel == null)
            {
                return;
            }

            await AddSession(selectedViewModel.OriginalModel);
        }

        private async Task OpenSession(SessionInfoDto sessionInfoDto)
        {
            _openWindowViewModel.IsOpenWindowVisible = false;
            await _telemetryLoadController.LoadRecentSessionAsync(sessionInfoDto);
        }

        private async Task AddSession(SessionInfoDto sessionInfoDto)
        {
            _addWindowViewModel.IsOpenWindowVisible = false;
            await _telemetryLoadController.AddRecentSessionAsync(sessionInfoDto);
        }

        private void TelemetryViewsSynchronizationOnNewSessionLoaded(object sender, TelemetrySessionArgs e)
        {
            _loadedSessions.Clear();
            _loadedSessions.Add(e.SessionInfoDto);
            _lastOpenedSession = e.SessionInfoDto;
        }

        private async Task ArchiveSession(IOpenWindowViewModel openWindowViewModel, SessionInfoDto sessionInfoDto)
        {
            try
            {
                openWindowViewModel.IsBusy = true;
                await _telemetryLoadController.ArchiveSession(sessionInfoDto);
                openWindowViewModel.RefreshRecentCommand.Execute(null);
                MessageBox.Show("Session Archived", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while archiving session");
                MessageBox.Show("Session Archivation failed", "Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                openWindowViewModel.IsBusy = false;
            }
        }


        private void BindCommands()
        {
            _openWindowViewModel.RefreshRecentCommand = new AsyncCommand(() => RefreshAllAvailableSession(_openWindowViewModel));
            _openWindowViewModel.CancelAndCloseWindowCommand = new RelayCommand(() => CancelAndCloseOpenWindow(_openWindowViewModel));
            _openWindowViewModel.OpenSelectedSessionCommand = new AsyncCommand(OpenSelectedRecentSession);

            _addWindowViewModel.RefreshRecentCommand = new AsyncCommand(() => RefreshAvailableSessionThatMatchTrack(_addWindowViewModel));
            _addWindowViewModel.CancelAndCloseWindowCommand = new RelayCommand(() => CancelAndCloseOpenWindow(_addWindowViewModel));
            _addWindowViewModel.OpenSelectedSessionCommand = new AsyncCommand(AddSelectedRecentSession);
        }
    }
}