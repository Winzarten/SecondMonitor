namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.LapPicker
{
    using Synchronization;

    public class LapPickerController : ILapPickerController
    {
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;

        public LapPickerController(ITelemetryViewsSynchronization telemetryViewsSynchronization)
        {
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
        }

        public void StartController()
        {
            Subscribe();
        }

        public void StopController()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            _telemetryViewsSynchronization.NewSessionLoaded += OnSessionStarted;
        }

        private void UnSubscribe()
        {
            _telemetryViewsSynchronization.NewSessionLoaded -= OnSessionStarted;
        }

        private void OnSessionStarted(object sender, TelemetrySessionArgs e)
        {
        }
    }
}