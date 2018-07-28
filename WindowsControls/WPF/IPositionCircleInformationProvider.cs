namespace SecondMonitor.WindowsControls.WPF
{
    using SecondMonitor.DataModel.Snapshot.Drivers;

    public interface IPositionCircleInformationProvider
    {
        bool IsDriverOnValidLap(DriverInfo driver);
    }
}