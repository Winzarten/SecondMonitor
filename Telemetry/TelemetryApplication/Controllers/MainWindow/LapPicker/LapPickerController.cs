namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.LapPicker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using AggregatedCharts;
    using Contracts.Commands;
    using Contracts.UserInput;
    using DataModel.Extensions;
    using OpenWindow;
    using SecondMonitor.ViewModels;
    using SecondMonitor.ViewModels.Colors;
    using SecondMonitor.ViewModels.Factory;
    using SettingsWindow;
    using Synchronization;
    using TelemetryLoad;
    using TelemetryManagement.DTO;
    using ViewModels;
    using ViewModels.GraphPanel;
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
        private readonly IUserInputProvider _userInputProvider;
        private readonly IAggregatedChartProvider _aggregatedChartProvider;
        private readonly ILapSelectionViewModel _lapSelectionViewModel;
        private readonly List<LapSummaryDto> _loadedLaps;

        public LapPickerController(ITelemetryViewsSynchronization telemetryViewsSynchronization, ITelemetryLoadController telemetryLoadController, IMainWindowViewModel mainWindowViewModel, IViewModelFactory viewModelFactory,
            ILapColorSynchronization lapColorSynchronization, IColorPaletteProvider colorPaletteProvider, IOpenWindowController openWindowController, ISettingsWindowController settingsWindowController, IUserInputProvider userInputProvider, IAggregatedChartProvider aggregatedChartProvider)
        {
            _loadedLaps = new List<LapSummaryDto>();
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _telemetryLoadController = telemetryLoadController;
            _lapSelectionViewModel = mainWindowViewModel.LapSelectionViewModel;
            _viewModelFactory = viewModelFactory;
            _lapColorSynchronization = lapColorSynchronization;
            _colorPaletteProvider = colorPaletteProvider;
            _openWindowController = openWindowController;
            _settingsWindowController = settingsWindowController;
            _userInputProvider = userInputProvider;
            _aggregatedChartProvider = aggregatedChartProvider;
        }

        public async Task StartControllerAsync()
        {
            Subscribe();
            _lapSelectionViewModel.AddCustomLapCommand = new AsyncCommand(AddCustomLap);
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
            _telemetryViewsSynchronization.SessionAdded += OnSessionAdded;
            _telemetryViewsSynchronization.LapAddedToSession += OnLapAddedToSession;
            _lapSelectionViewModel.LapSelected += LapSelectionViewModelOnLapSelected;
            _lapSelectionViewModel.LapUnselected += LapSelectionViewModelOnLapUnselected;
        }

        private void UnSubscribe()
        {
            _telemetryViewsSynchronization.NewSessionLoaded -= OnSessionStarted;
            _telemetryViewsSynchronization.SessionAdded -= OnSessionAdded;
            _telemetryViewsSynchronization.LapAddedToSession -= OnLapAddedToSession;
            _lapSelectionViewModel.LapSelected -= LapSelectionViewModelOnLapSelected;
            _lapSelectionViewModel.LapUnselected -= LapSelectionViewModelOnLapUnselected;
        }



        private void LapSelectionViewModelOnLapUnselected(object sender, LapSummaryArgs e)
        {
            _telemetryLoadController.UnloadLap(e.LapSummary);
        }

        private void LapSelectionViewModelOnLapSelected(object sender, LapSummaryArgs e)
        {
            _telemetryLoadController.LoadLap(e.LapSummary);
        }

        private void OnLapAddedToSession(object sender, LapSummaryArgs e)
        {
            AddLaps(new List<LapSummaryDto>(){ e.LapSummary});
        }

        private void AddLaps(IReadOnlyCollection<LapSummaryDto> lapsSummary)
        {
            foreach (LapSummaryDto lapSummaryDto in lapsSummary)
            {
                ILapSummaryViewModel newViewModel = _viewModelFactory.Create<ILapSummaryViewModel>();
                newViewModel.LapColorSynchronization = _lapColorSynchronization;
                newViewModel.FromModel(lapSummaryDto);
                newViewModel.LapColor = _colorPaletteProvider.GetNext();
                _lapSelectionViewModel.AddLapSummaryViewModel(newViewModel);
                _loadedLaps.Add(lapSummaryDto);
            }

            LapSummaryDto bestLap = _loadedLaps.OrderBy(x => x.LapTime).First();
            _lapSelectionViewModel.BestLap = $"{bestLap.CustomDisplayName} - {bestLap.LapTime.FormatToDefault()}";

            LapSummaryDto bestSector1Lap = _loadedLaps.OrderBy(x => x.Sector1Time).First();
            _lapSelectionViewModel.BestSector1 = bestSector1Lap.Sector1Time > TimeSpan.Zero ? $"{bestSector1Lap.CustomDisplayName} - {bestSector1Lap.Sector1Time.FormatToDefault()}" : string.Empty;

            LapSummaryDto bestSector2Lap = _loadedLaps.OrderBy(x => x.Sector1Time).First();
            _lapSelectionViewModel.BestSector2 = bestSector2Lap.Sector2Time > TimeSpan.Zero ? $"{bestSector2Lap.CustomDisplayName} - {bestSector2Lap.Sector2Time.FormatToDefault()}" : string.Empty;

            LapSummaryDto bestSector3Lap = _loadedLaps.OrderBy(x => x.Sector1Time).First();
            _lapSelectionViewModel.BestSector3 = bestSector3Lap.Sector3Time > TimeSpan.Zero ? $"{bestSector3Lap.CustomDisplayName} - {bestSector3Lap.Sector3Time.FormatToDefault()}" : string.Empty;
        }

        private void AddLapsFromSession(SessionInfoDto sessionInfoDto)
        {
            AddLaps(sessionInfoDto.LapsSummary);
        }

        private void ReinitializeViewMode(SessionInfoDto sessionInfoDto)
        {
            _lapSelectionViewModel.Clear();
            _loadedLaps.Clear();
            _lapSelectionViewModel.TrackName = string.IsNullOrEmpty(sessionInfoDto.LayoutName) ? sessionInfoDto.TrackName : $"{sessionInfoDto.TrackName} - {sessionInfoDto.LayoutName}";
            _lapSelectionViewModel.CarName = sessionInfoDto.CarName;
            _lapSelectionViewModel.SessionTime = sessionInfoDto.SessionRunDateTime;
            _lapSelectionViewModel.SimulatorName = sessionInfoDto.Simulator;
            AddLapsFromSession(sessionInfoDto);
        }

        private void OnSessionAdded(object sender, TelemetrySessionArgs e)
        {
            AddLapsFromSession(e.SessionInfoDto);
        }

        private async Task AddCustomLap()
        {

            AggregatedChartViewModel viewModel = _aggregatedChartProvider.CreateAggregatedChartViewModel();
            var win = new Window {Content = viewModel, Title = viewModel.Title, WindowState = WindowState.Maximized};
            win.Show();
            /*Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog {DefaultExt = ".lap", Filter = "Lap Files (*.lap)|*.lap"};
            bool? result = dlg.ShowDialog();
            if (result == false)
            {
                return;
            }

            string filename = dlg.FileName;
            string fileCustomName = await _userInputProvider.GetUserInput("Enter Lap Name:", $"Ex-{Path.GetFileNameWithoutExtension(filename)}");
            await _telemetryLoadController.LoadLap(new FileInfo(filename), fileCustomName);*/
        }


        private void OnSessionStarted(object sender, TelemetrySessionArgs e)
        {
            ReinitializeViewMode(e.SessionInfoDto);
        }
    }
}