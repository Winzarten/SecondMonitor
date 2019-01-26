namespace SecondMonitor.DataModel.Snapshot
{
    using System;

    using BasicProperties;

    [Serializable]
    public sealed class WeatherInfo
    {
        public WeatherInfo()
        {

        }

        public Temperature AirTemperature { get; set; } = Temperature.FromCelsius(-1);
        public Temperature TrackTemperature { get; set; } = Temperature.FromCelsius(-1);
        public int RainIntensity { get; set; } = 0;
    }
}
