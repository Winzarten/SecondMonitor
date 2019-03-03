namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.OpenWindow
{
    using System;
    using System.Windows.Input;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public interface IOpenWindowSessionInformationViewModel : IViewModel<SessionInfoDto>
    {
        DateTime SessionRunDateTime { get; }
        string SessionType { get; }
        string Simulator { get; }
        string TrackName { get; }
        string CarName { get; }
        int NumberOfLaps { get; }
        string PlayerName { get; }

        bool IsArchiveIconVisible { get; set; }
        ICommand ArchiveCommand { get; set; }
        ICommand SelectThisSessionCommand { get; set; }
        ICommand OpenSessionFolderCommand { get; set; }
        ICommand DeleteSessionCommand { get; set; }
    }
}