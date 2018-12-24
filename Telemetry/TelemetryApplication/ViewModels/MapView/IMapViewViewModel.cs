namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.MapView
{
    using WindowsControls.WPF.DriverPosition;
    using DataModel.Snapshot.Drivers;
    using DataModel.TrackMap;
    using SecondMonitor.ViewModels;

    public interface IMapViewViewModel : IAbstractViewModel
    {
        ISituationOverviewControl SituationOverviewControl { get; }

        void LoadTrack(ITrackMap trackMapDto);

        void RemoveDriver(IDriverInfo driverInfo);
        void UpdateDrivers(params IDriverInfo[] driversInfo);
    }
}