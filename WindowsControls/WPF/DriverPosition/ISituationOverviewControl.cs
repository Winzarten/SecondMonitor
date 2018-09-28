namespace SecondMonitor.WindowsControls.WPF.DriverPostion
{
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;

    public interface ISituationOverviewControl
    {
        bool AnimateDriversPos { get; set; }

        void AddDrivers(params DriverInfo[] drivers);

        void RemoveDrivers(params DriverInfo[] drivers);

        void UpdateDrivers(SimulatorDataSet dataSet, params DriverInfo[] drivers);

        void RemoveAllDrivers();
    }
}