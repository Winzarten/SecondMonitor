namespace SecondMonitor.DataModel.Snapshot
{
    using SecondMonitor.DataModel.BasicProperties;

    public class WeatherInfo
    {
        public Temperature AirTemperature { get; set; } = Temperature.FromCelsius(-1);
        public Temperature TrackTemperature { get; set; } = Temperature.FromCelsius(-1);
        public int RainIntensity { get; set; } = 0;
    }
}
