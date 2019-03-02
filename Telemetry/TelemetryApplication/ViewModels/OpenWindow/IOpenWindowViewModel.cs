namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.OpenWindow
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using SecondMonitor.ViewModels;

    public interface IOpenWindowViewModel : IViewModel
    {
        ICommand RefreshRecentCommand { get; set; }
        ICommand OpenSelectedSessionCommand { get; set; }
        int SelectedTabIndex { get; set; }
        IOpenWindowSessionInformationViewModel SelectedRecentSessionInfoDto { get; set; }
        IReadOnlyCollection<IOpenWindowSessionInformationViewModel> RecentSessionsInfos { get; set; }

        IOpenWindowSessionInformationViewModel SelectedArchiveSessionInfoDto { get; set; }
        IReadOnlyCollection<IOpenWindowSessionInformationViewModel> ArchiveSessionsInfos { get; set; }

        ICommand CancelAndCloseWindowCommand { get; set; }

        bool IsOpenWindowVisible { get; set; }
        bool IsBusy { get; set; }
    }
}