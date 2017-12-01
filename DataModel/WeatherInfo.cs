namespace SecondMonitor.DataModel
{
    public class WeatherInfo
    {
        public Temperature AirTemperature = Temperature.FromCelsius(-1);
        public Temperature TrackTemperature = Temperature.FromCelsius(-1);
        public int RainIntensity = 0;
    }
}
