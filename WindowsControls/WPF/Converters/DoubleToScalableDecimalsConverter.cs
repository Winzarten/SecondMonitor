namespace SecondMonitor.WindowsControls.WPF.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class DoubleToScalableDecimalsConverter : IValueConverter

    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double valueD)
            {
                if (double.IsInfinity(valueD) || double.IsNaN(valueD))
                {
                    return "-";
                }
                return valueD < 100 ? valueD.ToString("F1") : valueD.ToString("F0");
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}