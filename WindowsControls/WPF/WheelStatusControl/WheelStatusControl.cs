namespace SecondMonitor.WindowsControls.WPF.WheelStatusControl
{
    using System.Windows;
    using System.Windows.Controls;

    using SecondMonitor.DataModel.BasicProperties;

    public class WheelStatusControl : Control
    {
        private static readonly DependencyProperty TyreConditionProperty = DependencyProperty.Register("TyreCondition", typeof(double), typeof(WheelStatusControl));
        private static readonly DependencyProperty TyreCoreTemperatureProperty = DependencyProperty.Register("TyreCoreTemperature", typeof(OptimalQuantity<Temperature>), typeof(WheelStatusControl));
        private static readonly DependencyProperty BrakeTemperatureProperty = DependencyProperty.Register("BrakeTemperature", typeof(OptimalQuantity<Temperature>), typeof(WheelStatusControl));
        private static readonly DependencyProperty TyreSlippingIndicationProperty = DependencyProperty.Register("TyreSlippingIndication", typeof(bool), typeof(WheelStatusControl));


        public WheelStatusControl()
        {
            TyreCondition = 50.0;
        }

        public bool TyreSlippingIndication
        {
            get => (bool)GetValue(TyreSlippingIndicationProperty);
            set => SetValue(TyreSlippingIndicationProperty, value);
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

        public OptimalQuantity<Temperature> BrakeTemperature
        {
            get => (OptimalQuantity<Temperature>)GetValue(BrakeTemperatureProperty);
            set => SetValue(BrakeTemperatureProperty, value);
        }
    }
}