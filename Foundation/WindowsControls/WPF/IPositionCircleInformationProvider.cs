using System.Windows.Media;

namespace SecondMonitor.WindowsControls.WPF
{
    using DataModel.Snapshot.Drivers;

    public interface IPositionCircleInformationProvider
    {
        bool IsDriverOnValidLap(IDriverInfo driver);
        bool IsDriverLastSectorGreen(IDriverInfo driver, int sectorNumber);
        bool IsDriverLastSectorPurple(IDriverInfo driver, int sectorNumber);

        bool GetTryCustomOutline(IDriverInfo driverInfo, out SolidColorBrush outlineBrush);
    }
}