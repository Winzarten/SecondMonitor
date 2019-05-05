namespace SecondMonitor.WindowsControls.WPF.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using DataModel.BasicProperties;

    public class ColorDtoToSolidBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ColorDto colorDto))
            {
                return Colors.Transparent;
            }

            return colorDto.ToSolidColorBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SolidColorBrush soloColorBrush))
            {
                return null;
            }

            return ColorDto.FromColor(soloColorBrush.Color);
        }
    }
}