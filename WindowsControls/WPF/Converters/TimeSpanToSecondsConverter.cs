namespace SecondMonitor.WindowsControls.WPF.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class TimeSpanToSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan))
            {
                return string.Empty;
            }

            int decimalPlaces = (parameter != null) ? System.Convert.ToInt32(parameter) : 2;
            TimeSpan timeSpan = (TimeSpan)value;
            string convertedValue = timeSpan.TotalSeconds.ToString($"F{decimalPlaces}");
            return timeSpan > TimeSpan.Zero ? "+" + convertedValue : convertedValue;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}