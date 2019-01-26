namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.SettingsWindow
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using WindowsControls.WPF.Commands;
    using Factory;
    using Settings;
    using Settings.DTO;
    using ViewModels;
    using ViewModels.SettingsWindow;

    public class SettingsWindowController : ISettingsWindowController
    {
        private readonly ITelemetrySettingsRepository _telemetrySettingsRepository;
        private readonly ISettingsWindowViewModel _settingsWindowViewModel;

        public SettingsWindowController(IMainWindowViewModel mainWindowViewModel, IViewModelFactory viewModelFactory, ITelemetrySettingsRepository telemetrySettingsRepository)
        {
            _telemetrySettingsRepository = telemetrySettingsRepository;
            _settingsWindowViewModel = viewModelFactory.Create<ISettingsWindowViewModel>();
            mainWindowViewModel.LapSelectionViewModel.SettingsWindowViewModel = _settingsWindowViewModel;
            BindCommands();
        }

        public Task StartControllerAsync()
        {
            return Task.CompletedTask;
        }

        public Task StopControllerAsync()
        {
            return Task.CompletedTask;
        }

        private void OpenWindow()
        {
            TelemetrySettingsDto telemetrySettingsDto = _telemetrySettingsRepository.LoadOrCreateNew();
            _settingsWindowViewModel.FromModel(telemetrySettingsDto);
            _settingsWindowViewModel.IsWindowOpened = true;
        }

        private void SaveAndClose()
        {
            TelemetrySettingsDto newTelemetrySettingsDto = _settingsWindowViewModel.SaveToNewModel();
            _telemetrySettingsRepository.SaveTelemetrySettings(newTelemetrySettingsDto);
            CloseWindow();
        }

        private void CloseWindow()
        {
            _settingsWindowViewModel.IsWindowOpened = false;
        }

        private void BindCommands()
        {
            _settingsWindowViewModel.OpenWindowCommand = new RelayCommand(OpenWindow);
            _settingsWindowViewModel.CancelCommand = new RelayCommand(CloseWindow);
            _settingsWindowViewModel.OkCommand = new RelayCommand(SaveAndClose);
        }
    }
}