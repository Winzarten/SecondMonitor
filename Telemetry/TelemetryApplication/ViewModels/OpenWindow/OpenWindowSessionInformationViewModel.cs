namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.OpenWindow
{
    using System;
    using System.Windows.Input;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public class OpenWindowSessionInformationViewModel : AbstractViewModel<SessionInfoDto>, IOpenWindowSessionInformationViewModel
    {
        public DateTime SessionRunDateTime { get; set; }

        public string SessionType { get; set; }

        public string Simulator { get; set; }

        public string TrackName { get; set; }

        public string CarName { get; set; }

        public int NumberOfLaps { get; set; }

        public string PlayerName { get; set; }

        public bool IsArchiveIconVisible { get; set; }

        public ICommand ArchiveCommand { get; set; }

        public ICommand SelectThisSessionCommand { get; set; }

        public ICommand OpenSessionFolderCommand { get; set; }

        public ICommand DeleteSessionCommand { get; set; }

        protected override void ApplyModel(SessionInfoDto model)
        {
            SessionRunDateTime = model.SessionRunDateTime;
            SessionType = model.SessionType;
            Simulator = model.Simulator;
            TrackName = model.TrackName;
            CarName = model.CarName;
            NumberOfLaps = model.LapsSummary.Count;
            PlayerName = model.PlayerName;
        }

        public override SessionInfoDto SaveToNewModel()
        {
            throw new NotImplementedException();
        }
    }
}