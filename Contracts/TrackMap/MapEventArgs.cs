namespace SecondMonitor.Timing.Controllers
{
    using System;
    using DataModel.TrackMap;

    public class MapEventArgs : EventArgs
    {
        public MapEventArgs(TrackMapDto trackMapDto)
        {
            TrackMapDto = trackMapDto;
        }

        public TrackMapDto TrackMapDto { get; set; }
    }
}