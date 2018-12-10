namespace SecondMonitor.DataModel.TrackMap
{
    public interface ITrackMap
    {
        string TrackName { get; }
        string LayoutName { get; }
        string SimulatorSource { get; }
        TrackGeometryDto TrackGeometry { get; }
    }
}