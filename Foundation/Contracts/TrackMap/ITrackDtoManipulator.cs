namespace SecondMonitor.Contracts.TrackMap
{
    using DataModel.TrackMap;

    public interface ITrackDtoManipulator
    {
        TrackMapDto RotateLeft(TrackMapDto trackMapDto);
        TrackMapDto RotateRight(TrackMapDto trackMapDto);
    }
}