namespace SecondMonitor.WindowsControls.WPF.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class TimeSpanToBrushConverter : IValueConverter
    {
        private static readonly Brush NegativeBrush = Brushes.Green;
        private static readonly Brush PositiveBrush = new SolidColorBrush(Color.FromRgb(189, 7, 59));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan))
            {
                return Brushes.White;
            }
            TimeSpan timeSpan = (TimeSpan)value;
            return timeSpan < TimeSpan.Zero ? NegativeBrush : PositiveBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}