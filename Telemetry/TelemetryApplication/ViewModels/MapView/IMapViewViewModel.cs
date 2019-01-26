namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.MapView
{
    using System;
    using System.Threading.Tasks;
    using WindowsControls.WPF.DriverPosition;
    using Controllers.Synchronization;
    using DataModel.Snapshot.Drivers;
    using DataModel.TrackMap;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public interface IMapViewViewModel : IViewModel, IDisposable
    {
        FullMapControl SituationOverviewControl { get; }
        ILapColorSynchronization LapColorSynchronization { get; set; }

        bool? ShowAllOverlays { get; set; }
        bool ShowBrakeOverlay { get; set; }
        bool ShowThrottleOverlay { get; set; }
        bool ShowClutchOverlay { get; set; }
        bool ShowShiftPoints { get; set; }

        void LoadTrack(ITrackMap trackMapDto);

        void RemoveDriver(IDriverInfo driverInfo);
        void UpdateDrivers(params IDriverInfo[] driversInfo);
        Task AddPathsForLap(LapTelemetryDto lapTelemetry, TrackMapDto trackMapDto);
        void RemovePathsForLap(LapSummaryDto lapTelemetry);



    }
}