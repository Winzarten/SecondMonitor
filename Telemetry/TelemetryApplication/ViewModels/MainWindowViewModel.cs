namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels
{
    using Factory;
    using LapPicker;
    using SecondMonitor.ViewModels;

    public class MainWindowViewModel : AbstractViewModel, IMainWindowViewModel
    {
        private readonly IViewModelFactory _viewModelFactory;

        public MainWindowViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            LapSelectionViewModel = viewModelFactory.Create<ILapSelectionViewModel>();
        }

        public ILapSelectionViewModel LapSelectionViewModel { get; }
    }
}