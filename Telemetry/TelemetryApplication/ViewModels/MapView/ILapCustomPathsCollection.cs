namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.MapView
{
    using System.Windows.Shapes;

    public interface ILapCustomPathsCollection
    {
        string LapId { get; set; }
        bool FullyInitialized { get; set; }

        Path ShiftPointsPath { get; set; }

        Path BaseLapPath { get; set; }
        void AddBrakingPath(Path path, double intensity);
        Path GetBrakingPath(double intensity);

        void AddClutchPath(Path path, double intensity);

        void AddThrottlePath(Path path, double intensity);
        Path GetThrottlePath(double intensity);

        Path[] GetAllPaths();
    }
}