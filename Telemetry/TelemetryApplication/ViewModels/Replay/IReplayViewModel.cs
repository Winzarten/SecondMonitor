namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.Replay
{
    using System;
    using DataModel.BasicProperties;
    using SecondMonitor.ViewModels;

    public interface IReplayViewModel : IAbstractViewModel, IDisableViewModel
    {
        double TrackLengthRaw { get; }
        Distance TrackLength { get; set; }
        Distance DisplayDistance { get; set; }
        TimeSpan DisplayTime { get; set; }
        DistanceUnits DistanceUnits { get; set; }
        double SelectedDistance { get; set; }
    }
}