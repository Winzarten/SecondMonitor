namespace SecondMonitor.WindowsControls.WPF.WheelStatusControl
{
    using System.Windows;
    using System.Windows.Controls;

    using SecondMonitor.DataModel.BasicProperties;

    public class WheelStatusControl : Control
    {
        private static readonly DependencyProperty TemperatureUnitsProperty = DependencyProperty.Register("TemperatureUnits", typeof(TemperatureUnits), typeof(WheelStatusControl));
        private static readonly DependencyProperty PressureUnitsProperty = DependencyProperty.Register("PressureUnits", typeof(PressureUnits), typeof(WheelStatusControl));
        private static readonly DependencyProperty TyreConditionProperty = DependencyProperty.Register("TyreCondition", typeof(double), typeof(WheelStatusControl));
        private static readonly DependencyProperty TyreCoreTemperatureProperty = DependencyProperty.Register("TyreCoreTemperature", typeof(OptimalQuantity<Temperature>), typeof(WheelStatusControl));
        private static readonly DependencyProperty TyreLeftTemperatureProperty = DependencyProperty.Register("TyreLeftTemperature", typeof(OptimalQuantity<Temperature>), typeof(WheelStatusControl));
        private static readonly DependencyProperty TyreCenterTemperatureProperty = DependencyProperty.Register("TyreCenterTemperature", typeof(OptimalQuantity<Temperature>), typeof(WheelStatusControl));
        private static readonly DependencyProperty TyreRightTemperatureProperty = DependencyProperty.Register("TyreRightTemperature", typeof(OptimalQuantity<Temperature>), typeof(WheelStatusControl));
        private static readonly DependencyProperty BrakeTemperatureProperty = DependencyProperty.Register("BrakeTemperature", typeof(OptimalQuantity<Temperature>), typeof(WheelStatusControl), new PropertyMetadata(){ PropertyChangedCallback = PropertyChangedCallback });

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static readonly DependencyProperty TyreSlippingIndicationProperty = DependencyProperty.Register("TyreSlippingIndication", typeof(bool), typeof(WheelStatusControl));
        private static readonly DependencyProperty TyrePressureProperty = DependencyProperty.Register("TyrePressure", typeof(OptimalQuantity<Pressure>), typeof(WheelStatusControl));
        private static readonly DependencyProperty IsLeftWheelProperty = DependencyProperty.Register("IsLeftWheel", typeof(bool), typeof(WheelStatusControl));


        public WheelStatusControl()
        {
            TyreCondition = 50.0;
        }

        public bool TyreSlippingIndication
        {
            get => (bool)GetValue(TyreSlippingIndicationProperty);
            set => SetValue(TyreSlippingIndicationProperty, value);
        }

        public TemperatureUnits TemperatureUnits
        {
            get => (TemperatureUnits)GetValue(TemperatureUnitsProperty);
            set => SetValue(TemperatureUnitsProperty, value);
        }

        public PressureUnits PressureUnits
        {
            get => (PressureUnits)GetValue(PressureUnitsProperty);
            set => SetValue(PressureUnitsProperty, value);
        }

        public double TyreCondition
        {
            get => (double)GetValue(TyreConditionProperty);
            set => SetValue(TyreConditionProperty, value);
        }

        public OptimalQuantity<Temperature> TyreCoreTemperature
        {
            get => (OptimalQuantity<Temperature>)GetValue(TyreCoreTemperatureProperty);
            set => SetValue(TyreCoreTemperatureProperty, value);
        }

        public OptimalQuantity<Temperature> TyreLeftTemperature
        {
            get => (OptimalQuantity<Temperature>)GetValue(TyreLeftTemperatureProperty);
            set => SetValue(TyreLeftTemperatureProperty, value);
        }

        public OptimalQuantity<Temperature> TyreCenterTemperature
        {
            get => (OptimalQuantity<Temperature>)GetValue(TyreCenterTemperatureProperty);
            set => SetValue(TyreCenterTemperatureProperty, value);
        }

        public OptimalQuantity<Temperature> TyreRightTemperature
        {
            get => (OptimalQuantity<Temperature>)GetValue(TyreRightTemperatureProperty);
            set => SetValue(TyreRightTemperatureProperty, value);
        }

        public OptimalQuantity<Temperature> BrakeTemperature
        {
            get => (OptimalQuantity<Temperature>)GetValue(BrakeTemperatureProperty);
            set => SetValue(BrakeTemperatureProperty, value);
        }

        public OptimalQuantity<Pressure> TyrePressure
        {
            get => (OptimalQuantity<Pressure>)GetValue(TyrePressureProperty);
            set => SetValue(TyrePressureProperty, value);
        }

        public bool IsLeftWheel
        {
            get => (bool)GetValue(IsLeftWheelProperty);
            set => SetValue(IsLeftWheelProperty, value);
        }
    }
}