namespace SecondMonitor.WindowsControls.WPF.DriverPostion
{
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;

    public interface ISituationOverviewControl
    {
        bool AnimateDriversPos { get; set; }

        void AddDrivers(params DriverInfo[] drivers);

        void RemoveDrivers(params DriverInfo[] drivers);

        void UpdateDrivers(SimulatorDataSet dataSet, params DriverInfo[] drivers);

        void RemoveAllDrivers();
    }
}