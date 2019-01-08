namespace SecondMonitor.Timing.SessionTiming.Drivers.ViewModel
{
    public enum LapInvalidationReasonKind
    {
        NoValidLapTime,
        CompletedDistanceLessThanLapThreshold,
        NotAllSectorHaveTime,
        SanityMaxSpeedViolated,
        InvalidatedBySectorTime,
        InvalidatedFirstLap,
        DriverDnf,
        DriverInPits,
        InvalidatedBySim
    }
}