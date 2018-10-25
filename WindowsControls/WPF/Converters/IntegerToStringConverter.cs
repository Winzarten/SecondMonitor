namespace SecondMonitor.WindowsControls.WPF.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class IntegerToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue.ToString();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (int.TryParse(stringValue, out var toReturn))
                {
                    return toReturn;
                }
            }

            return 0;
        }
    }
}