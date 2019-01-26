namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.SettingsWindow
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using SecondMonitor.ViewModels;
    using Settings.DTO;

    public interface ISettingsWindowViewModel : IViewModel<TelemetrySettingsDto>
    {
        bool IsWindowOpened { get; set; }
        ObservableCollection<IGraphSettingsViewModel> LeftPanelGraphs { get; }
        ObservableCollection<IGraphSettingsViewModel> RightPanelGraphs { get; }
        ObservableCollection<IGraphSettingsViewModel> NotUsedGraphs { get;  }
        XAxisKind XAxisKind { get; set; }

        ICommand OpenWindowCommand { get; set; }
        ICommand CancelCommand { get; set; }
        ICommand OkCommand { get; set; }
    }
}