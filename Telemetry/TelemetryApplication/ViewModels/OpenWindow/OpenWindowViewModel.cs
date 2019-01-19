namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.OpenWindow
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public class OpenWindowViewModel : AbstractViewModel, IOpenWindowViewModel
    {
        private ICommand _refreshCommand;
        private ICommand _openSelectedRecentSessionCommand;
        private SessionInfoDto _selectedSessionInfoDto;
        private ICommand _cancelAndCloseWindowCommand;
        private IReadOnlyCollection<SessionInfoDto> _recentSessionsInfos;
        private bool _openWindowVisible;
        private bool _isBusy;

        public ICommand RefreshRecentCommand
        {
            get => _refreshCommand;
            set => SetProperty(ref _refreshCommand, value);
        }

        public ICommand OpenSelectedRecentSessionCommand
        {
            get => _openSelectedRecentSessionCommand;
            set => SetProperty(ref _openSelectedRecentSessionCommand, value);
        }

        public SessionInfoDto SelectedRecentSessionInfoDto
        {
            get => _selectedSessionInfoDto;
            set => SetProperty(ref _selectedSessionInfoDto, value);
        }

        public IReadOnlyCollection<SessionInfoDto> RecentSessionsInfos
        {
            get => _recentSessionsInfos;
            set => SetProperty(ref _recentSessionsInfos, value);
        }

        public ICommand CancelAndCloseWindowCommand
        {
            get => _cancelAndCloseWindowCommand;
            set => SetProperty(ref _cancelAndCloseWindowCommand, value);
        }

        public bool IsOpenWindowVisible
        {
            get => _openWindowVisible;
            set => SetProperty(ref _openWindowVisible,  value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
    }
}