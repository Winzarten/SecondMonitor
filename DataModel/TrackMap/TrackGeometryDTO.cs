using System;

namespace SecondMonitor.DataModel.TrackMap
{
    [Serializable]
    public class TrackGeometryDto : ITrackGeometry
    {
        public int ExporterVersion { get; set; }
        public double LeftOffset { get; set; }
        public double TopOffset { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string FullMapGeometry { get; set; }
        public string Sector1Geometry { get; set; }
        public string Sector2Geometry { get; set; }
        public string Sector3Geometry { get; set; }
        public string StartLineGeometry { get; set; }
    }
}