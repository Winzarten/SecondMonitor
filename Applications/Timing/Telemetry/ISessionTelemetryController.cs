﻿namespace SecondMonitor.Timing.Telemetry
{
    using System.Threading.Tasks;
    using SessionTiming.Drivers.ViewModel;

    public interface ISessionTelemetryController
    {
        Task SaveLapTelemetry(LapInfo lapInfo);
    }
}