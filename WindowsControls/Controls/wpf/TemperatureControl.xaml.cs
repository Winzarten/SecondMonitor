using SecondMonitor.DataModel;
using System.Windows;
using System.Windows.Controls;

namespace SecondMonitor.WindowsControls.Controls.wpf
{
    using SecondMonitor.DataModel.BasicProperties;

    /// <inheritdoc cref="UserControl" />
    /// <summary>
    /// Interaction logic for WaterTemp.xaml
    /// </summary>
    public partial class TemperatureControl : UserControl
    {
        public static readonly DependencyProperty DisplayUnitsProperty = DependencyProperty.Register("DisplayUnits", typeof(TemperatureUnits), typeof(TemperatureControl), new PropertyMetadata(TemperatureUnits.Celsius, PropertyChangedCallback ));

        public TemperatureControl()
        {
            InitializeComponent();
        }
        
        public TemperatureUnits DisplayUnits
        {
            get => (TemperatureUnits) GetValue(DisplayUnitsProperty);
            set => SetValue(DisplayUnitsProperty, value);
        }

        private Temperature _temperature = Temperature.FromCelsius(0);
        private Temperature _minimumTemperature = Temperature.FromCelsius(0);
        private Temperature _maximumTemperature = Temperature.FromCelsius(120);
        private Temperature _redTemperatureStart = Temperature.FromCelsius(110);

        public double MaximumTemperatureCelsius
        {
            get => _maximumTemperature.InCelsius;
            set
            {
                _maximumTemperature = Temperature.FromCelsius(value);
                ChangeDisplayUnit();
            }
        }

        public double MinimumTemperatureCelsius
        {
            get => _minimumTemperature.InCelsius;
            set
            {
                _minimumTemperature = Temperature.FromCelsius(value);
                ChangeDisplayUnit();
            }
        }

        public float ScaleLinesMajorStepValue
        {
            get => waterGauge.ScaleLinesMajorStepValue;
            set => waterGauge.ScaleLinesMajorStepValue = value;
        }

        public double RedTemperatureStart
        {
            get => _redTemperatureStart.InCelsius;
            set
            {
                _redTemperatureStart = Temperature.FromCelsius(value);
                ChangeDisplayUnit();
            }
        }

        public Temperature Temperature
        {
            get { return _temperature; }
            set
            {
                if (_temperature == value)
                    return;
                _temperature = value;
                var displayValue = value.GetValueInUnits(DisplayUnits);
                lock (waterGauge)
                {
                    waterGauge.Value = (float)displayValue;
                    waterGauge.GaugeLabels[0].Text = displayValue.ToString("N1") + Temperature.GetUnitSymbol(DisplayUnits);
                    if (_temperature < _redTemperatureStart)
                    {
                        waterGauge.GaugeLabels[0].Color = System.Drawing.Color.Green;
                    }
                    else
                    {
                        waterGauge.GaugeLabels[0].Color = System.Drawing.Color.Red;
                    }
                }

            }
        }

        private void ChangeDisplayUnit()
        {
            lock (waterGauge)
            {
                waterGauge.MaxValue = 1000;
                waterGauge.MinValue = -1000;
                waterGauge.GaugeRanges[0].EndValue = 1000;
                waterGauge.GaugeRanges[0].StartValue = -1000;
                waterGauge.Value = 500;
                waterGauge.GaugeRanges[0].EndValue = (float)_maximumTemperature.GetValueInUnits(DisplayUnits);
                waterGauge.GaugeRanges[0].StartValue = (float)_redTemperatureStart.GetValueInUnits(DisplayUnits);
                waterGauge.MaxValue = (float)_maximumTemperature.GetValueInUnits(DisplayUnits);
                waterGauge.MinValue = (float)_minimumTemperature.GetValueInUnits(DisplayUnits);
                var displayValue = _temperature.GetValueInUnits(DisplayUnits);
                waterGauge.GaugeLabels[0].Text = displayValue.ToString("N1") + Temperature.GetUnitSymbol(DisplayUnits);
                waterGauge.Update();
            }
        }
        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            TemperatureControl control = (TemperatureControl) dependencyObject;
            control.ChangeDisplayUnit();
        }
    }
}
