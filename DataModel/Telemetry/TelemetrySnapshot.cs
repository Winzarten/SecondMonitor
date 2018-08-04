using SecondMonitor.DataModel.Snapshot;
using SecondMonitor.DataModel.Snapshot.Drivers;

namespace SecondMonitor.DataModel.Telemetry
{
    public class TelemetrySnapshot
    {
        public TelemetrySnapshot(DriverInfo playerInfo, WeatherInfo weatherInfo)
        {
            PlayerData = playerInfo;
            WeatherInfo = weatherInfo;
        }

        public DriverInfo PlayerData { get; }
        public WeatherInfo WeatherInfo { get; }
    }
}