namespace SecondMonitor.WindowsControls.WPF.PedalControl
{
    using System.Windows;
    using System.Windows.Controls;
    using DataModel.BasicProperties;

    public class PedalGearControl : Control
    {
        private static readonly DependencyProperty ThrottlePercentageProperty = DependencyProperty.Register("ThrottlePercentage",typeof(double), typeof(PedalGearControl));
        private static readonly DependencyProperty BrakePercentageProperty = DependencyProperty.Register("BrakePercentage", typeof(double), typeof(PedalGearControl));
        private static readonly DependencyProperty ClutchPercentageProperty = DependencyProperty.Register("ClutchPercentage", typeof(double), typeof(PedalGearControl));
        private static readonly DependencyProperty WheelRotationProperty = DependencyProperty.Register("WheelRotation", typeof(double), typeof(PedalGearControl));
        private static readonly DependencyProperty GearProperty = DependencyProperty.Register("Gear", typeof(string), typeof(PedalGearControl));
        public static readonly DependencyProperty VelocityUnitsProperty = DependencyProperty.Register("VelocityUnits", typeof(VelocityUnits), typeof(PedalGearControl), new PropertyMetadata(default(VelocityUnits)));
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(Velocity), typeof(PedalGearControl), new PropertyMetadata(Velocity.FromKph(0)));
        public static readonly DependencyProperty RpmProperty = DependencyProperty.Register("Rpm", typeof(int), typeof(PedalGearControl), new PropertyMetadata(default(int)));

        public int Rpm
        {
            get => (int) GetValue(RpmProperty);
            set => SetValue(RpmProperty, value);
        }

        public Velocity Speed
        {
            get => (Velocity) GetValue(SpeedProperty);
            set => SetValue(SpeedProperty, value);
        }

        public VelocityUnits VelocityUnits
        {
            get => (VelocityUnits) GetValue(VelocityUnitsProperty);
            set => SetValue(VelocityUnitsProperty, value);
        }

        public double WheelRotation
        {
            get => (double)GetValue(WheelRotationProperty);
            set => SetValue(WheelRotationProperty, value);
        }

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