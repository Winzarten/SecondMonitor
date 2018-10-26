namespace SecondMonitor.WindowsControls.WPF.FuelControl
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using DataModel.BasicProperties;
    using DataModel.BasicProperties.FuelConsumption;

    public class FuelPlannerControl : Control
    {
        private static readonly DependencyProperty VolumeUnitsProperty = DependencyProperty.Register("VolumeUnits", typeof(VolumeUnits), typeof(FuelPlannerControl));
        private static readonly DependencyProperty DistanceUnitsProperty = DependencyProperty.Register("DistanceUnits", typeof(DistanceUnits), typeof(FuelPlannerControl));
        private static readonly DependencyProperty FuelPerDistanceUnitsProperty = DependencyProperty.Register("FuelPerDistanceUnits", typeof(FuelPerDistanceUnits), typeof(FuelPlannerControl));
        private static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(FuelPlannerControl));

        public ICommand CloseCommand
        {
            get => (ICommand) GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public VolumeUnits VolumeUnits
        {
            get => (VolumeUnits)GetValue(VolumeUnitsProperty);
            set => SetValue(VolumeUnitsProperty, value);
        }

        public FuelPerDistanceUnits FuelPerDistanceUnits
        {
            get => (FuelPerDistanceUnits)GetValue(FuelPerDistanceUnitsProperty);
            set => SetValue(FuelPerDistanceUnitsProperty, value);
        }

        public DistanceUnits DistanceUnits
        {
            get => (DistanceUnits)GetValue(DistanceUnitsProperty);
            set => SetValue(DistanceUnitsProperty, value);
        }
    }
}