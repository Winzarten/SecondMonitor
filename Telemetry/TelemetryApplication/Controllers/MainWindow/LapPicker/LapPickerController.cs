namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.LapPicker
{
    using System.Linq;
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
            _lapSelectionViewModel.TrackName = string.IsNullOrEmpty(sessionInfoDto.LayoutName) ? sessionInfoDto.TrackName : $"{sessionInfoDto.TrackName} - {sessionInfoDto.LayoutName}";
            _lapSelectionViewModel.CarName = sessionInfoDto.CarName;
            _lapSelectionViewModel.SessionTime = sessionInfoDto.SessionRunDateTime;
            _lapSelectionViewModel.SimulatorName = sessionInfoDto.Simulator;
            foreach (LapSummaryDto lapSummaryDto in sessionInfoDto.LapsSummary.OrderBy(x => x.LapNumber))
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