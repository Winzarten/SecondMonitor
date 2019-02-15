namespace SecondMonitor.Contracts.TrackMap
{
    using System;
    using DataModel.TrackMap;

    public interface IMapManagementController
    {
        event EventHandler<MapEventArgs> NewMapAvailable;
        event EventHandler<MapEventArgs> MapRemoved;

        bool TryGetMap(string simulator, string trackName, string layoutName, out TrackMapDto trackMapDto);

        void RemoveMap(string simulator, string trackName, string layoutName);
        TrackMapDto RotateMapRight(string simulator, string trackName, string layoutName);
        TrackMapDto RotateMapLeft(string simulator, string trackName, string layoutName);

        TimeSpan MapPointsInterval { get; set; }

    }
}