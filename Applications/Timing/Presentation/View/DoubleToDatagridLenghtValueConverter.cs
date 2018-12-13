namespace SecondMonitor.Timing.Presentation.View
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class DoubleToDataGridLengthValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? DataGridLength.Auto : new DataGridLength((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DataGridLength?)value)?.Value ?? 0;
        }
    }
}