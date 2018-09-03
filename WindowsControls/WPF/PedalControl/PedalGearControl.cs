namespace SecondMonitor.WindowsControls.WPF.PedalControl
{
    using System.Windows;
    using System.Windows.Controls;

    public class PedalGearControl : Control
    {
        private static readonly DependencyProperty ThrottlePercentageProperty = DependencyProperty.Register("ThrottlePercentage",typeof(double), typeof(PedalGearControl));
        private static readonly DependencyProperty BrakePercentageProperty = DependencyProperty.Register("BrakePercentage", typeof(double), typeof(PedalGearControl));
        private static readonly DependencyProperty ClutchPercentageProperty = DependencyProperty.Register("ClutchPercentage", typeof(double), typeof(PedalGearControl));
        private static readonly DependencyProperty GearProperty = DependencyProperty.Register("Gear", typeof(string), typeof(PedalGearControl));

        public double ThrottlePercentage
        {
            get => (double)GetValue(ThrottlePercentageProperty);
            set => SetValue(ThrottlePercentageProperty, value);
        }

        public double BrakePercentage
        {
            get => (double)GetValue(BrakePercentageProperty);
            set => SetValue(BrakePercentageProperty, value);
        }

        public double ClutchPercentage
        {
            get => (double)GetValue(ClutchPercentageProperty);
            set => SetValue(ClutchPercentageProperty, value);
        }

        public string Gear
        {
            get => (string)GetValue(GearProperty);
            set => SetValue(GearProperty, value);
        }
    }
}