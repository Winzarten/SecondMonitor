namespace SecondMonitor.WindowsControls.WPF.WheelStatusControl
{
    using System.Windows;
    using System.Windows.Controls;

    public class WheelStatusControl : Control
    {
        private static readonly DependencyProperty TyreConditionProperty = DependencyProperty.Register("TyreCondition", typeof(double), typeof(WheelStatusControl));

        public WheelStatusControl()
        {
            TyreCondition = 50.0;
        }

        public double TyreCondition
        {
            get => (double)GetValue(TyreConditionProperty);
            set => SetValue(TyreConditionProperty, value);
        }
    }
}