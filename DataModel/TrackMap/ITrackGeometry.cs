namespace SecondMonitor.DataModel.TrackMap
{
    public interface ITrackGeometry
    {
        int ExporterVersion { get; }

        double LeftOffset { get; }

        double TopOffset { get; }

        double Width { get; }

        double Height { get; }

        string FullMapGeometry { get; }

        string Sector1Geometry { get; }

        string Sector2Geometry { get; }

        string Sector3Geometry { get; }

        string StartLineGeometry { get; }
    }
}