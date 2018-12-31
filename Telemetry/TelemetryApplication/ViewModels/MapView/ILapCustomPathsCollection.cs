namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.MapView
{
    using System.Collections.Generic;
    using System.Windows.Shapes;

    public interface ILapCustomPathsCollection
    {
        string LapId { get; set; }
        bool FullyInitialized { get; set; }

        Path ShiftPointsPath { get; set; }

        Path BaseLapPath { get; set; }
        void AddBrakingPath(Path path, double intensity);
        Path GetBrakingPath(double intensity);
        IEnumerable<Path> GetAllBrakingPaths();

        void AddClutchPath(Path path, double intensity);
        IEnumerable<Path> GetAllClutchPaths();

        void AddThrottlePath(Path path, double intensity);
        Path GetThrottlePath(double intensity);
        IEnumerable<Path> GetAllThrottlePaths();

        Path[] GetAllPaths();
    }
}