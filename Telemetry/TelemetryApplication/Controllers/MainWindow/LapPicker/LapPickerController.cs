namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.LapPicker
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DataModel.Extensions;
    using Factory;
    using OpenWindow;
    using SecondMonitor.ViewModels.Colors;
    using SettingsWindow;
    using Synchronization;
    using TelemetryLoad;
    using TelemetryManagement.DTO;
    using ViewModels;
    using ViewModels.LapPicker;

    public class LapPickerController : ILapPickerController
    {
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;
        private readonly ITelemetryLoadController _telemetryLoadController;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly ILapColorSynchronization _lapColorSynchronization;
        private readonly IColorPaletteProvider _colorPaletteProvider;
        private readonly IOpenWindowController _openWindowController;
        private readonly ISettingsWindowController _settingsWindowController;
        private readonly ILapSelectionViewModel _lapSelectionViewModel;

        public LapPickerController(ITelemetryViewsSynchronization telemetryViewsSynchronization, ITelemetryLoadController telemetryLoadController, IMainWindowViewModel mainWindowViewModel, IViewModelFactory viewModelFactory,
            ILapColorSynchronization lapColorSynchronization, IColorPaletteProvider colorPaletteProvider, IOpenWindowController openWindowController, ISettingsWindowController settingsWindowController)
        {
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _telemetryLoadController = telemetryLoadController;
            _lapSelectionViewModel = mainWindowViewModel.LapSelectionViewModel;
            _viewModelFactory = viewModelFactory;
            _lapColorSynchronization = lapColorSynchronization;
            _colorPaletteProvider = colorPaletteProvider;
            _openWindowController = openWindowController;
            _settingsWindowController = settingsWindowController;
        }

        public async Task StartControllerAsync()
        {
            Subscribe();
            await StartChildControllersAsync();
        }

        public async Task StopControllerAsync()
        {
            UnSubscribe();
            await StopChildControllersAsync();
        }

        private async Task StartChildControllersAsync()
        {
            await _openWindowController.StartControllerAsync();
            await _settingsWindowController.StartControllerAsync();
        }

        private async Task StopChildControllersAsync()
        {
            await _openWindowController.StopControllerAsync();
            await _settingsWindowController.StopControllerAsync();
        }

        private void Subscribe()
        {
            _telemetryViewsSynchronization.NewSessionLoaded += OnSessionStarted;
            _lapSelectionViewModel.LapSelected += LapSelectionViewModelOnLapSelected;
            _lapSelectionViewModel.LapUnselected += LapSelectionViewModelOnLapUnselected;
        }

        private void UnSubscribe()
        {
            _telemetryViewsSynchronization.NewSessionLoaded -= OnSessionStarted;
            _lapSelectionViewModel.LapSelected -= LapSelectionViewModelOnLapSelected;
            _lapSelectionViewModel.LapUnselected -= LapSelectionViewModelOnLapUnselected;
        }

        private void LapSelectionViewModelOnLapUnselected(object sender, LapSummaryArgs e)
        {
            _telemetryLoadController.UnloadLap(e.LapSummary);
        }

        private void LapSelectionViewModelOnLapSelected(object sender, LapSummaryArgs e)
        {
            _telemetryLoadController.LoadLap(e.LapSummary.LapNumber);
        }

        private void ReinitializeViewMode(SessionInfoDto sessionInfoDto)
        {
            _lapSelectionViewModel.Clear();
            _lapSelectionViewModel.TrackName = string.IsNullOrEmpty(sessionInfoDto.LayoutName) ? sessionInfoDto.TrackName : $"{sessionInfoDto.TrackName} - {sessionInfoDto.LayoutName}";
            _lapSelectionViewModel.CarName = sessionInfoDto.CarName;
            _lapSelectionViewModel.SessionTime = sessionInfoDto.SessionRunDateTime;
            _lapSelectionViewModel.SimulatorName = sessionInfoDto.Simulator;
            LapSummaryDto bestLap = sessionInfoDto.LapsSummary.OrderBy(x => x.LapTime).First();
            _lapSelectionViewModel.BestLap = $"{bestLap.LapNumber} - {bestLap.LapTime.FormatToDefault()}";

            LapSummaryDto bestSector1Lap = sessionInfoDto.LapsSummary.OrderBy(x => x.Sector1Time).First();
            _lapSelectionViewModel.BestSector1 = bestSector1Lap.Sector1Time > TimeSpan.Zero ? $"{bestSector1Lap.LapNumber} - {bestSector1Lap.Sector1Time.FormatToDefault()}" : string.Empty;

            LapSummaryDto bestSector2Lap = sessionInfoDto.LapsSummary.OrderBy(x => x.Sector1Time).First();
            _lapSelectionViewModel.BestSector2 = bestSector2Lap.Sector2Time > TimeSpan.Zero ? $"{bestSector2Lap.LapNumber} - {bestSector2Lap.Sector2Time.FormatToDefault()}" : string.Empty;

            LapSummaryDto bestSector3Lap = sessionInfoDto.LapsSummary.OrderBy(x => x.Sector1Time).First();
            _lapSelectionViewModel.BestSector3 = bestSector3Lap.Sector3Time > TimeSpan.Zero ? $"{bestSector3Lap.LapNumber} - {bestSector3Lap.Sector3Time.FormatToDefault()}" : string.Empty;

            foreach (LapSummaryDto lapSummaryDto in sessionInfoDto.LapsSummary.OrderBy(x => x.LapNumber))
            {
                ILapSummaryViewModel newViewModel = _viewModelFactory.Create<ILapSummaryViewModel>();
                newViewModel.LapColorSynchronization = _lapColorSynchronization;
                newViewModel.FromModel(lapSummaryDto);
                newViewModel.LapColor = _colorPaletteProvider.GetNext();
                _lapSelectionViewModel.AddLapSummaryViewModel(newViewModel);
            }
        }

        private void OnSessionStarted(object sender, TelemetrySessionArgs e)
        {
            ReinitializeViewMode(e.SessionInfoDto);
        }
    }
}