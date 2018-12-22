namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.Replay
{
    using Factory;
    using Synchronization;
    using TelemetryManagement.DTO;
    using ViewModels.SnapshotSection;

    public class ReplayController : IReplayController
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;

        public ReplayController(IViewModelFactory viewModelFactory, ITelemetryViewsSynchronization telemetryViewsSynchronization)
        {
            _viewModelFactory = viewModelFactory;
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
        }

        public LapSummaryDto MainLap { get; set; }

        public ISnapshotSectionViewModel SnapshotSectionViewModel { get; set; }

        public void StartController()
        {

        }

        public void StopController()
        {

        }


    }
}