namespace SecondMonitor.Contracts.TrackMap
{
    using System;
    using DataModel.TrackMap;
    using Timing.Controllers;

    public interface IMapManagementController
    {
        event EventHandler<MapEventArgs> NewMapAvailable;
        bool TryGetMap(string simulator, string trackName, string layoutName, out TrackMapDto trackMapDto);
    }
}