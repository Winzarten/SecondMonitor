namespace SecondMonitor.ViewModels.CarStatus
{
    using System;

    public interface IPaceProvider
    {
        TimeSpan? PlayersPace { get; }
        TimeSpan? LeadersPace { get; }
    }
}