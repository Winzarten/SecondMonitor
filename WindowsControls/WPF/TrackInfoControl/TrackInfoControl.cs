namespace SecondMonitor.WindowsControls.WPF.TrackInfoControl
{
    using System.Windows;
    using System.Windows.Controls;

    public class TrackInfoControl : Control
    {
        private static readonly DependencyProperty TrackNameProperty = DependencyProperty.Register("TrackName", typeof(string), typeof(TrackInfoControl));
        private static readonly DependencyProperty SessionTypeProperty = DependencyProperty.Register("SessionType", typeof(string), typeof(TrackInfoControl));
        private static readonly DependencyProperty WeatherInfoAvailableProperty = DependencyProperty.Register("WeatherInfoAvailable", typeof(bool), typeof(TrackInfoControl));
        private static readonly DependencyProperty RainIntensityInfoProperty = DependencyProperty.Register("RainIntensityInfo", typeof(string), typeof(TrackInfoControl));
        private static readonly DependencyProperty AirTemperatureInfoProperty = DependencyProperty.Register("AirTemperatureInfo", typeof(string), typeof(TrackInfoControl));
        private static readonly DependencyProperty TrackTemperatureInfoProperty = DependencyProperty.Register("TrackTemperatureInfo", typeof(string), typeof(TrackInfoControl));

        public string TrackName
        {
            get => (string)GetValue(TrackNameProperty);
            set => SetValue(TrackNameProperty, value);
        }

        public bool WeatherInfoAvailable
        {
            get => (bool)GetValue(WeatherInfoAvailableProperty);
            set => SetValue(WeatherInfoAvailableProperty, value);
        }

        public string SessionType
        {
            get => (string)GetValue(SessionTypeProperty);
            set => SetValue(SessionTypeProperty, value);
        }

        public string RainIntensityInfo
        {
            get => (string)GetValue(RainIntensityInfoProperty);
            set => SetValue(RainIntensityInfoProperty, value);
        }

        public string AirTemperatureInfo
        {
            get => (string)GetValue(AirTemperatureInfoProperty);
            set => SetValue(AirTemperatureInfoProperty, value);
        }

        public string TrackTemperatureInfo
        {
            get => (string)GetValue(TrackTemperatureInfoProperty);
            set => SetValue(TrackTemperatureInfoProperty, value);
        }
    }
}