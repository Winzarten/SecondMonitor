namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.Replay
{
    using System;
    using System.Windows.Input;
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
        Distance SyncDistance { get; set; }

        ICommand StopCommand { get; set; }
        ICommand PlayCommand { get; set; }
        ICommand NextFrameCommand { get; set; }
        ICommand PreviousFrameCommand { get; set; }
        ICommand SyncDistancesCommand { get; set; }
    }
}