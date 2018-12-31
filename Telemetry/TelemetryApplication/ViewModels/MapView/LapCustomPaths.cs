namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.MapView
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Shapes;

    public class LapCustomPathsCollection : ILapCustomPathsCollection
    {
        private readonly Dictionary<double, Path> _brakingPaths;
        private readonly Dictionary<double, Path> _throttlePaths;
        private readonly Dictionary<double, Path> _clutchPaths;

        public LapCustomPathsCollection()
        {
            _brakingPaths = new Dictionary<double, Path>();
            _throttlePaths = new Dictionary<double, Path>();
            _clutchPaths = new Dictionary<double, Path>();
        }

        public string LapId { get; set; }

        public bool FullyInitialized { get; set; }

        public Path ShiftPointsPath { get; set; }

        public Path BaseLapPath { get; set; }

        public void AddBrakingPath(Path path, double intensity)
        {
            _brakingPaths[intensity] = path;
        }

        public Path GetBrakingPath(double intensity)
        {
            return _brakingPaths[intensity];
        }

        public IEnumerable<Path> GetAllBrakingPaths() => _brakingPaths.Values;


        public void AddClutchPath(Path path, double intensity)
        {
            _clutchPaths[intensity] = path;
        }

        public IEnumerable<Path> GetAllClutchPaths() => _clutchPaths.Values;


        public void AddThrottlePath(Path path, double intensity)
        {
            _throttlePaths[intensity] = path;
        }

        public Path GetThrottlePath(double intensity)
        {
            return _throttlePaths[intensity];
        }

        public IEnumerable<Path> GetAllThrottlePaths() => _throttlePaths.Values;


        public Path[] GetAllPaths()
        {
            return _brakingPaths.Values.Concat(_throttlePaths.Values.Concat(_clutchPaths.Values).Append(BaseLapPath).Append(ShiftPointsPath)).ToArray();
        }
    }
}