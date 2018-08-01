namespace SecondMonitor.WindowsControls.WPF
{
    using DataModel.Snapshot.Drivers;

    public interface IPositionCircleInformationProvider
    {
        bool IsDriverOnValidLap(DriverInfo driver);
    }
}