namespace SecondMonitor.WindowsControls.WPF.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class TimeSpanToSecondsNoSignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan))
            {
                return string.Empty;
            }

            int decimalPlaces = (parameter != null) ? System.Convert.ToInt32(parameter) : 2;
            TimeSpan timeSpan = (TimeSpan)value;
            return timeSpan.TotalSeconds.ToString($"F{decimalPlaces}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}