namespace SecondMonitor.WindowsControls.WPF.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    public class TimeSpanToMinutesSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan valueSpan)
            {
                string seconds = valueSpan.Seconds.ToString("00").Replace("-", string.Empty);
                return $"{(int)valueSpan.TotalMinutes}:{seconds}";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}