namespace SecondMonitor.WindowsControls.WPF.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using DataModel.BasicProperties;

    public class TemperatureToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Temperature) || !(parameter is TemperatureUnits))
            {
                return 0;
            }

            Temperature temperature = (Temperature)value;
            TemperatureUnits temperatureUnits = (TemperatureUnits)parameter;
            return temperature.GetValueInUnits(temperatureUnits);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}