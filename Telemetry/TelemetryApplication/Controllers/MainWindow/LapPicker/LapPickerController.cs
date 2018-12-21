namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.LapPicker
{
    using Factory;
    using Synchronization;
    using TelemetryManagement.DTO;
    using ViewModels;
    using ViewModels.LapPicker;

    public class LapPickerController : ILapPickerController
    {
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly ILapSelectionViewModel _lapSelectionViewModel;

        public LapPickerController(ITelemetryViewsSynchronization telemetryViewsSynchronization, IMainWindowViewModel mainWindowViewModel, IViewModelFactory viewModelFactory)
        {
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _lapSelectionViewModel = mainWindowViewModel.LapSelectionViewModel;
            _viewModelFactory = viewModelFactory;
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

        private void ReinitializeViewMode(SessionInfoDto sessionInfoDto)
        {
            _lapSelectionViewModel.Clear();
            foreach (LapSummaryDto lapSummaryDto in sessionInfoDto.LapsSummary)
            {
                ILapSummaryViewModel newViewModel = _viewModelFactory.Create<ILapSummaryViewModel>();
                newViewModel.FromModel(lapSummaryDto);
                _lapSelectionViewModel.AddLapSummaryViewModel(newViewModel);
            }
        }

        private void OnSessionStarted(object sender, TelemetrySessionArgs e)
        {
            ReinitializeViewMode(e.SessionInfoDto);
        }
    }
}