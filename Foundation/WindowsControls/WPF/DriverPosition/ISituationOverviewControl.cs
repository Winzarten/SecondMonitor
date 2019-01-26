using SecondMonitor.DataModel.Snapshot;
using SecondMonitor.DataModel.Snapshot.Drivers;

namespace SecondMonitor.WindowsControls.WPF.DriverPosition
{
    public interface ISituationOverviewControl
    {
        bool AnimateDriversPos { get; set; }

        string AdditionalInformation { get; set; }

        IPositionCircleInformationProvider PositionCircleInformationProvider { get; set; }

        void AddDrivers(params IDriverInfo[] drivers);

        void RemoveDrivers(params IDriverInfo[] drivers);

        void UpdateDrivers(SimulatorDataSet dataSet, params IDriverInfo[] drivers);

        void RemoveAllDrivers();
    }
}