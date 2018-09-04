namespace SecondMonitor.WindowsControls.WPF.TemperatureControl
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;

    using SecondMonitor.DataModel.BasicProperties;

    public class TemperatureControl : Control, INotifyPropertyChanged
    {
        private static readonly DependencyProperty TemperatureUnitsProperty = DependencyProperty.Register("TemperatureUnits", typeof(TemperatureUnits), typeof(TemperatureControl), new PropertyMetadata { DefaultValue = TemperatureUnits.Celsius, PropertyChangedCallback = TemperatureUnitsChanged });
        private static readonly DependencyProperty MinimalTemperatureProperty = DependencyProperty.Register("MinimalTemperature", typeof(Temperature), typeof(TemperatureControl), new PropertyMetadata { DefaultValue = Temperature.FromCelsius(0), PropertyChangedCallback = BoundaryTemperaturePropertyChangedCallback });
        private static readonly DependencyProperty TemperatureProperty = DependencyProperty.Register("Temperature", typeof(Temperature), typeof(TemperatureControl), new PropertyMetadata {DefaultValue = Temperature.FromCelsius(30), PropertyChangedCallback = TemperaturePropertyChangedCallback });
        private static readonly DependencyProperty MaximalNormalTemperatureProperty = DependencyProperty.Register("MaximalNormalTemperature", typeof(Temperature), typeof(TemperatureControl), new PropertyMetadata(Temperature.FromCelsius(110)));
        private static readonly DependencyProperty MaximalTemperatureProperty = DependencyProperty.Register("MaximalTemperature", typeof(Temperature), typeof(TemperatureControl),  new PropertyMetadata {DefaultValue = Temperature.FromCelsius(140), PropertyChangedCallback = BoundaryTemperaturePropertyChangedCallback });
        private static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(Viewbox), typeof(TemperatureControl));

        private string _formattedValue;
        private double _valueInProperUnits;
        private double _gaugeLowerTemperature;
        private double _gaugeUpperTemperature;
        private double _gaugeMaxNormalTemperature;

        public event PropertyChangedEventHandler PropertyChanged;

        public TemperatureControl()
        {
            RefreshFormattedValue();
            RefreshLimits();
        }

        public TemperatureUnits TemperatureUnits
        {
            get => (TemperatureUnits)GetValue(TemperatureUnitsProperty);
            set => SetValue(TemperatureUnitsProperty, value);
        }

        public Temperature MinimalTemperature
        {
            get => (Temperature)GetValue(MinimalTemperatureProperty);
            set => SetValue(MinimalTemperatureProperty, value);
        }

        public Temperature Temperature
        {
            get => (Temperature)GetValue(TemperatureProperty);
            set => SetValue(TemperatureProperty, value);
        }

        public Temperature MaximalNormalTemperature
        {
            get => (Temperature)GetValue(MaximalNormalTemperatureProperty);
            set => SetValue(MaximalNormalTemperatureProperty, value);
        }

        public Temperature MaximalTemperature
        {
            get => (Temperature)GetValue(MaximalTemperatureProperty);
            set => SetValue(MaximalTemperatureProperty, value);
        }

        public Viewbox Icon
        {
            get => (Viewbox)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string FormattedValue
        {
            get => _formattedValue;
            set
            {
                _formattedValue = value;
                NotifyPropertyChanged();
            }
        }

        public double ValueInProperUnits
        {
            get => _valueInProperUnits;
            set
            {
                _valueInProperUnits = value;
                NotifyPropertyChanged();
            }
        }

        public double GaugeLowerTemperature
        {
            get => _gaugeLowerTemperature;
            set
            {
                _gaugeLowerTemperature = value;
                NotifyPropertyChanged();
            }
        }

        public double GaugeUpperTemperature
        {
            get => _gaugeUpperTemperature;
            set
            {
                _gaugeUpperTemperature = value;
                NotifyPropertyChanged();
            }
        }

        public double GaugeMaxNormalTemperature
        {
            get => _gaugeMaxNormalTemperature;
            set
            {
                _gaugeMaxNormalTemperature = value;
                NotifyPropertyChanged();
            }
        }

        public int LabelStep => TemperatureUnits != TemperatureUnits.Fahrenheit ? 20  : 40;

        public TimeSpan GaugeSpeed => TimeSpan.FromSeconds(1);


        private void RefreshFormattedValue()
        {
            if (Temperature == null)
            {
                FormattedValue = string.Empty;
                return;
            }

            ValueInProperUnits = Temperature.GetValueInUnits(TemperatureUnits);
            FormattedValue = ValueInProperUnits.ToString("F1") + Temperature.GetUnitSymbol(TemperatureUnits);
        }

        private void RefreshLimits()
        {

            if (MinimalTemperature != null && MaximalTemperature != null)
            {
                double newGaugeLower =  Math.Round(MinimalTemperature.GetValueInUnits(TemperatureUnits) / 10.0) * 10;
                double newGaugeUpper = Math.Round(MaximalTemperature.GetValueInUnits(TemperatureUnits) / 10.0) * 10;
                if (newGaugeLower < newGaugeUpper)
                {
                    GaugeLowerTemperature = -10;
                    GaugeUpperTemperature = 2000;
                    GaugeLowerTemperature = newGaugeLower;
                    GaugeUpperTemperature = newGaugeUpper;
                }
            }

            if (MaximalNormalTemperature != null)
            {
                GaugeMaxNormalTemperature = MaximalNormalTemperature.GetValueInUnits(TemperatureUnits);
            }

            RefreshFormattedValue();
        }

        private static void TemperaturePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TemperatureControl control)
            {
                control.RefreshFormattedValue();
            }
        }

        private static void TemperatureUnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TemperatureControl control)
            {
                control.RefreshFormattedValue();
                control.RefreshLimits();
            }
        }

        private static void BoundaryTemperaturePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TemperatureControl control)
            {
                control.RefreshLimits();
            }
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}