namespace SecondMonitor.WindowsControls.WPF.FuelControl
{
    using System.Windows;
    using System.Windows.Controls;
    using DataModel.BasicProperties;

    public class FuelCalculatorControl : Control
    {
        private static readonly DependencyProperty RequiredLapsProperty = DependencyProperty.Register("RequiredLaps", typeof(int), typeof(FuelCalculatorControl));
        private static readonly DependencyProperty RequiredMinutesProperty = DependencyProperty.Register("RequiredMinutes", typeof(int), typeof(FuelCalculatorControl));
        private static readonly DependencyProperty RequiredFuelProperty = DependencyProperty.Register("RequiredFuel", typeof(Volume), typeof(FuelCalculatorControl));
        private static readonly DependencyProperty VolumeUnitsProperty = DependencyProperty.Register("VolumeUnits", typeof(VolumeUnits), typeof(FuelCalculatorControl));

        public VolumeUnits VolumeUnits
        {
            get => (VolumeUnits) GetValue(VolumeUnitsProperty);
            set => SetValue(VolumeUnitsProperty, value);
        }

        public int RequiredLaps
        {
            get => (int)GetValue(RequiredLapsProperty);
            set => SetValue(RequiredLapsProperty, value);
        }

        public int RequiredMinutes
        {
            get => (int)GetValue(RequiredMinutesProperty);
            set => SetValue(RequiredMinutesProperty, value);
        }

        public Volume RequiredFuel
        {
            get => (Volume)GetValue(RequiredFuelProperty);
            set => SetValue(RequiredFuelProperty, value);
        }
    }
}