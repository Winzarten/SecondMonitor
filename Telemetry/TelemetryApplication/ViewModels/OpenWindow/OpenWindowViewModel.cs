namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.OpenWindow
{
    using System.Windows.Input;
    using SecondMonitor.ViewModels;

    public class OpenWindowViewModel : AbstractViewModel, IOpenWindowViewModel
    {
        private ICommand _refreshCommand;

        public ICommand RefreshRecentCommand
        {
            get => _refreshCommand;
            set => SetProperty(ref _refreshCommand, value);
        }
    }
}