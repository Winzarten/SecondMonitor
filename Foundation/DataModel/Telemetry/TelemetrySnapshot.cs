namespace SecondMonitor.DataModel.Telemetry
{
    using System;
    using BasicProperties;
    using Snapshot;
    using Snapshot.Drivers;

    [Serializable]
    public class TelemetrySnapshot
    {
        public TelemetrySnapshot()
        {

        }

        public TelemetrySnapshot(DriverInfo playerInfo, WeatherInfo weatherInfo, InputInfo inputInfo, SimulatorSourceInfo simulatorSourceInfo)
        {
            PlayerData = playerInfo;
            WeatherInfo = weatherInfo;
            InputInfo = inputInfo;
            SimulatorSourceInfo = simulatorSourceInfo;
        }

        public DriverInfo PlayerData { get; set; }
        public WeatherInfo WeatherInfo { get; set; }
        public InputInfo InputInfo { get; set; }
        public SimulatorSourceInfo SimulatorSourceInfo { get; set; }
    }
}