namespace SecondMonitor.WindowsControls.WPF
{
    using DataModel.BasicProperties;
    using DataModel.Snapshot.Drivers;

    public interface IPositionCircleInformationProvider
    {
        bool IsDriverOnValidLap(IDriverInfo driver);
        bool IsDriverLastSectorGreen(IDriverInfo driver, int sectorNumber);
        bool IsDriverLastSectorPurple(IDriverInfo driver, int sectorNumber);

        bool TryGetCustomOutline(IDriverInfo driverInfo, out ColorDto outlineColor);
        ColorDto GetClassColor(IDriverInfo driverInfo);

    }
}