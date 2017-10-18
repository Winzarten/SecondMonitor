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
            ChangeDisplayUnit();            
        }

        private Temperature.TemperatureUnits displayUnit;
        public Temperature.TemperatureUnits DisplayUnit { get => displayUnit;
            set
            {
                displayUnit = value;               
            }
        }

        private Temperature temperature = Temperature.FromCelsius(0);
        private Temperature maximumTemperature = Temperature.FromCelsius(120);
        private Temperature redTemperatureStart = Temperature.FromCelsius(100);

        public double MaximumTemperatureCelsius
        {
            get => maximumTemperature.InCelsius;
            set
            {
                maximumTemperature = Temperature.FromCelsius(value);
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
            get => redTemperatureStart.InCelsius;
            set
            {
                redTemperatureStart = Temperature.FromCelsius(value);
                ChangeDisplayUnit();
            }
        }

        public Temperature Temperature
        {
            get { return temperature; }
            set
            {
                if (temperature == value)
                    return;
                temperature = value;
                var displayValue = GetDisplayUnitValue(value);
                waterGauge.Value = (float)displayValue;
                waterGauge.GaugeLabels[0].Text = displayValue.ToString("N1") + UnitLabel();
                if(temperature < redTemperatureStart)
                {
                    waterGauge.GaugeLabels[0].Color = System.Drawing.Color.Green;
                }else
                {
                    waterGauge.GaugeLabels[0].Color = System.Drawing.Color.Red;
                }

            }
        }

        private void ChangeDisplayUnit()
        {

            waterGauge.MaxValue = (float)GetDisplayUnitValue(maximumTemperature);
            waterGauge.GaugeRanges[0].EndValue = waterGauge.MaxValue;
            waterGauge.GaugeRanges[0].StartValue = (float)GetDisplayUnitValue(redTemperatureStart);
        }

        public double GetDisplayUnitValue(Temperature temperature)
        {

            if (DisplayUnit == Temperature.TemperatureUnits.Ceslius)
                return temperature.InCelsius;
            else
                return temperature.InFahrenheit;

        }
        private string UnitLabel()
        {
            if (DisplayUnit == Temperature.TemperatureUnits.Ceslius)
                return "°C";
            else
                return "°F";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeDisplayUnit();
        }
    }
}
