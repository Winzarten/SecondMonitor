namespace SecondMonitor.WindowsControls.WPF
{
    using DataModel.Snapshot.Drivers;

    public interface IPositionCircleInformationProvider
    {
        bool IsDriverOnValidLap(DriverInfo driver);
        bool IsDriverLastSectorGreen(DriverInfo driver, int sectorNumber);
        bool IsDriverLastSectorPurple(DriverInfo driver, int sectorNumber);
    }
}