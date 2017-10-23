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

        private TemperatureUnits displayUnit;
        public TemperatureUnits DisplayUnit { get => displayUnit;
            set
            {
                if (displayUnit == value)
                    return;
                displayUnit = value;
                ChangeDisplayUnit();
            }
        }

        private Temperature temperature = Temperature.FromCelsius(0);
        private Temperature minimumTemperature = Temperature.FromCelsius(0);
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

        public double MinimumTemperatureCelsius
        {
            get => minimumTemperature.InCelsius;
            set
            {
                minimumTemperature = Temperature.FromCelsius(value);
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
                var displayValue = value.GetValueInUnits(displayUnit);
                lock (waterGauge)
                {
                    waterGauge.Value = (float)displayValue;
                    waterGauge.GaugeLabels[0].Text = displayValue.ToString("N1") + Temperature.GetUnitSymbol(displayUnit);
                    if (temperature < redTemperatureStart)
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
                waterGauge.GaugeRanges[0].EndValue = (float)maximumTemperature.GetValueInUnits(displayUnit);
                waterGauge.GaugeRanges[0].StartValue = (float)redTemperatureStart.GetValueInUnits(displayUnit);
                waterGauge.MaxValue = (float)maximumTemperature.GetValueInUnits(displayUnit);
                waterGauge.MinValue = (float)minimumTemperature.GetValueInUnits(displayUnit);
                var displayValue = temperature.GetValueInUnits(displayUnit);
                waterGauge.GaugeLabels[0].Text = displayValue.ToString("N1") + Temperature.GetUnitSymbol(displayUnit);
                waterGauge.Update();
            }
        }
        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {            
        }
    }
}
