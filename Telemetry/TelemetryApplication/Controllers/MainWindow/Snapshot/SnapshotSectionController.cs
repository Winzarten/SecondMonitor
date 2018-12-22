namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.Snapshot
{
    using Factory;
    using Replay;
    using ViewModels.SnapshotSection;

    public class SnapshotSectionController : ISnapshotSectionController
    {
        private readonly IReplayController _replayController;
        private readonly ISnapshotSectionViewModel _snapshotSectionViewModel;

        public SnapshotSectionController(IReplayController replayController, IViewModelFactory viewModelFactory)
        {
            _replayController = replayController;
            _snapshotSectionViewModel = viewModelFactory.Create<ISnapshotSectionViewModel>();
        }

        public void StartController()
        {
            StartChildControllers();
        }

        public void StopController()
        {
            StopChildControllers();
        }

        private void StartChildControllers()
        {
            _replayController.SnapshotSectionViewModel = _snapshotSectionViewModel;
            _replayController.StartController();
        }

        private void StopChildControllers()
        {
            _replayController.StopController();
        }
    }
}