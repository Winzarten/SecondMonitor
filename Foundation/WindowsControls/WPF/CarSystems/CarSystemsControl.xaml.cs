using System.Windows.Controls;

namespace SecondMonitor.WindowsControls.WPF.CarSystems
{
    using System.Windows;
    using DataModel.BasicProperties;

    /// <summary>
    /// Interaction logic for CarSystemsControl.xaml
    /// </summary>
    public partial class CarSystemsControl : UserControl
    {

        public static readonly DependencyProperty PressureUnitsProperty = DependencyProperty.Register("PressureUnits", typeof(PressureUnits), typeof(CarSystemsControl), new PropertyMetadata(default(PressureUnits)));
        public static readonly DependencyProperty TemperatureUnitsProperty = DependencyProperty.Register("TemperatureUnits", typeof(TemperatureUnits), typeof(CarSystemsControl), new PropertyMetadata(default(TemperatureUnits)));

        public TemperatureUnits TemperatureUnits
        {
            get => (TemperatureUnits) GetValue(TemperatureUnitsProperty);
            set => SetValue(TemperatureUnitsProperty, value);
        }

        public PressureUnits PressureUnits
        {
            get => (PressureUnits) GetValue(PressureUnitsProperty);
            set => SetValue(PressureUnitsProperty, value);
        }

        public CarSystemsControl()
        {
            InitializeComponent();
        }
    }
}
