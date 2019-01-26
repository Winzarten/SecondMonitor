namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.OpenWindow
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public interface IOpenWindowViewModel : IViewModel
    {
        ICommand RefreshRecentCommand { get; set; }
        ICommand OpenSelectedRecentSessionCommand { get; set; }
        SessionInfoDto SelectedRecentSessionInfoDto { get; set; }
        IReadOnlyCollection<SessionInfoDto> RecentSessionsInfos { get; set; }
        ICommand CancelAndCloseWindowCommand { get; set; }
        bool IsOpenWindowVisible { get; set; }
        bool IsBusy { get; set; }
    }
}