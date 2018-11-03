using SecondMonitor.DataModel.Snapshot;
using SecondMonitor.DataModel.Snapshot.Drivers;

namespace SecondMonitor.WindowsControls.WPF.DriverPosition
{
    public interface ISituationOverviewControl
    {
        bool AnimateDriversPos { get; set; }

        string AdditionalInformation { get; set; }

        IPositionCircleInformationProvider PositionCircleInformationProvider { get; set; }

        void AddDrivers(params DriverInfo[] drivers);

        void RemoveDrivers(params DriverInfo[] drivers);

        void UpdateDrivers(SimulatorDataSet dataSet, params DriverInfo[] drivers);

        void RemoveAllDrivers();
    }
}