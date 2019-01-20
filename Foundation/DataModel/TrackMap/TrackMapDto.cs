namespace SecondMonitor.DataModel.TrackMap
{
    public sealed class TrackMapDto : ITrackMap
    {
        public string TrackName { get; set; }
        public string LayoutName { get; set; }
        public string SimulatorSource { get; set; }
        public TrackGeometryDto TrackGeometry { get; set; }
    }
}