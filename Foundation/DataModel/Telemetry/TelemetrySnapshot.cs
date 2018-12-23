namespace SecondMonitor.DataModel.Telemetry
{
    using BasicProperties;
    using Snapshot;
    using Snapshot.Drivers;

    public class TelemetrySnapshot
    {
        public TelemetrySnapshot()
        {

        }

        public TelemetrySnapshot(DriverInfo playerInfo, WeatherInfo weatherInfo, InputInfo inputInfo)
        {
            PlayerData = playerInfo;
            WeatherInfo = weatherInfo;
            InputInfo = inputInfo;
        }

        public DriverInfo PlayerData { get; set; }
        public WeatherInfo WeatherInfo { get; set; }
        public InputInfo InputInfo { get; set; }
    }
}