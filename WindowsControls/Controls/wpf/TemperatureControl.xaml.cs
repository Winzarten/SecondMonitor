using SecondMonitor.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecondMonitor.WindowsControls.Controls.wpf
{
    /// <summary>
    /// Interaction logic for WaterTemp.xaml
    /// </summary>
    public partial class TemperatureControl : UserControl
    {
        

        public TemperatureControl()
        {
            InitializeComponent();            
        }

        private TemperatureUnits _displayUnit;
        public TemperatureUnits DisplayUnit { get => _displayUnit;
            set
            {
                if (_displayUnit == value)
                    return;
                _displayUnit = value;
                ChangeDisplayUnit();
            }
        }

        private Temperature _temperature = Temperature.FromCelsius(0);
        private Temperature _minimumTemperature = Temperature.FromCelsius(0);
        private Temperature _maximumTemperature = Temperature.FromCelsius(120);
        private Temperature _redTemperatureStart = Temperature.FromCelsius(100);

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
                var displayValue = value.GetValueInUnits(_displayUnit);
                lock (waterGauge)
                {
                    waterGauge.Value = (float)displayValue;
                    waterGauge.GaugeLabels[0].Text = displayValue.ToString("N1") + Temperature.GetUnitSymbol(_displayUnit);
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
                waterGauge.GaugeRanges[0].EndValue = (float)_maximumTemperature.GetValueInUnits(_displayUnit);
                waterGauge.GaugeRanges[0].StartValue = (float)_redTemperatureStart.GetValueInUnits(_displayUnit);
                waterGauge.MaxValue = (float)_maximumTemperature.GetValueInUnits(_displayUnit);
                waterGauge.MinValue = (float)_minimumTemperature.GetValueInUnits(_displayUnit);
                var displayValue = _temperature.GetValueInUnits(_displayUnit);
                waterGauge.GaugeLabels[0].Text = displayValue.ToString("N1") + Temperature.GetUnitSymbol(_displayUnit);
                waterGauge.Update();
            }
        }
        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {            
        }
    }
}
