namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.OpenWindow
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using SecondMonitor.ViewModels;

    public interface IOpenWindowViewModel : IViewModel
    {
        ICommand RefreshRecentCommand { get; set; }
        ICommand OpenSelectedRecentSessionCommand { get; set; }
        IOpenWindowSessionInformationViewModel SelectedRecentSessionInfoDto { get; set; }
        IReadOnlyCollection<IOpenWindowSessionInformationViewModel> RecentSessionsInfos { get; set; }
        ICommand CancelAndCloseWindowCommand { get; set; }
        bool IsOpenWindowVisible { get; set; }
        bool IsBusy { get; set; }
    }
}