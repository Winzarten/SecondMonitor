namespace SecondMonitor.WindowsControls.WPF.DriverPosition
{
    using System.Windows.Input;

    public interface IMapSidePanelViewModel
    {
        ICommand DeleteMapCommand { get; }
        ICommand RotateMapLeftCommand { get; }
        ICommand RotateMapRightCommand { get; }
    }
}