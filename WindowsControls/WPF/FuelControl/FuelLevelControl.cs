namespace SecondMonitor.WindowsControls.WPF.FuelControl
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class FuelLevelControl : Control
    {
        private static readonly DependencyProperty FuelPercentageProperty = DependencyProperty.Register("FuelPercentage",typeof(double), typeof(FuelLevelControl));
        private static readonly DependencyProperty IconColorProperty = DependencyProperty.Register("IconColor", typeof(SolidColorBrush), typeof(FuelLevelControl));


        public double FuelPercentage
        {
            get => (double)GetValue(FuelPercentageProperty);
            set => SetValue(FuelPercentageProperty, value);
        }

        public SolidColorBrush IconColor
        {
            get => (SolidColorBrush)GetValue(IconColorProperty);
            set => SetValue(IconColorProperty, value);
        }
    }
}