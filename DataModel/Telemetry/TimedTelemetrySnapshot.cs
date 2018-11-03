using System;
using SecondMonitor.DataModel.Snapshot;
using SecondMonitor.DataModel.Snapshot.Drivers;

namespace SecondMonitor.DataModel.Telemetry
{
    public class TimedTelemetrySnapshot : TelemetrySnapshot
    {
        public TimedTelemetrySnapshot(TimeSpan lapTime, DriverInfo playerInfo, WeatherInfo weatherInfo) : base(playerInfo, weatherInfo)
        {
            LapTime = lapTime;
        }

        public TimeSpan LapTime { get; }
    }
}