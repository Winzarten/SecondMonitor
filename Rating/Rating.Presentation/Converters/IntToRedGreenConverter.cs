namespace SecondMonitor.Rating.Presentation.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class IntToRedGreenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue < 0 ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff4d4d")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF1D"));
            }

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}