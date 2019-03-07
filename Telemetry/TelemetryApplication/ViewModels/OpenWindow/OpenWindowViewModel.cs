namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.OpenWindow
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using SecondMonitor.ViewModels;

    public class OpenWindowViewModel : AbstractViewModel, IOpenWindowViewModel
    {
        private ICommand _refreshCommand;
        private ICommand _openSelectedRecentSessionCommand;
        private IOpenWindowSessionInformationViewModel _selectedSessionInfoDto;
        private ICommand _cancelAndCloseWindowCommand;
        private IReadOnlyCollection<IOpenWindowSessionInformationViewModel> _recentSessionsInfos;
        private bool _openWindowVisible;
        private bool _isBusy;
        private IOpenWindowSessionInformationViewModel _selectedArchiveSessionInfoDto;
        private IReadOnlyCollection<IOpenWindowSessionInformationViewModel> _archiveSessionsInfos;
        private int _selectedTabIndex;

        public ICommand RefreshRecentCommand
        {
            get => _refreshCommand;
            set => SetProperty(ref _refreshCommand, value);
        }

        public ICommand OpenSelectedSessionCommand
        {
            get => _openSelectedRecentSessionCommand;
            set => SetProperty(ref _openSelectedRecentSessionCommand, value);
        }

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        public IOpenWindowSessionInformationViewModel SelectedRecentSessionInfoDto
        {
            get => _selectedSessionInfoDto;
            set => SetProperty(ref _selectedSessionInfoDto, value);
        }

        public IReadOnlyCollection<IOpenWindowSessionInformationViewModel> RecentSessionsInfos
        {
            get => _recentSessionsInfos;
            set => SetProperty(ref _recentSessionsInfos, value);
        }

        public IOpenWindowSessionInformationViewModel SelectedArchiveSessionInfoDto
        {
            get => _selectedArchiveSessionInfoDto;
            set => SetProperty(ref _selectedArchiveSessionInfoDto, value);
        }

        public IReadOnlyCollection<IOpenWindowSessionInformationViewModel> ArchiveSessionsInfos
        {
            get => _archiveSessionsInfos;
            set => SetProperty(ref _archiveSessionsInfos, value);
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