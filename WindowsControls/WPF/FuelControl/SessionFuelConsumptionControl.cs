namespace SecondMonitor.WindowsControls.WPF.FuelControl
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using DataModel.BasicProperties;
    using DataModel.BasicProperties.FuelConsumption;

    public class SessionFuelConsumptionControl : Control
    {
        private static readonly DependencyProperty TrackNameProperty = DependencyProperty.Register("TrackName", typeof(string), typeof(SessionFuelConsumptionControl));
        private static readonly DependencyProperty SessionTypeProperty = DependencyProperty.Register("SessionType", typeof(string), typeof(SessionFuelConsumptionControl));
        private static readonly DependencyProperty TraveledDistanceProperty = DependencyProperty.Register("TraveledDistance", typeof(Distance), typeof(SessionFuelConsumptionControl));
        private static readonly DependencyProperty RunningTimeProperty = DependencyProperty.Register("RunningTime", typeof(TimeSpan), typeof(SessionFuelConsumptionControl));
        private static readonly DependencyProperty TotalConsumedFuelProperty = DependencyProperty.Register("TotalConsumedFuel", typeof(Volume), typeof(SessionFuelConsumptionControl));
        private static readonly DependencyProperty AvgPerMinuteProperty = DependencyProperty.Register("AvgPerMinute", typeof(Volume), typeof(SessionFuelConsumptionControl));
        private static readonly DependencyProperty AvgPerLapProperty = DependencyProperty.Register("AvgPerLap", typeof(Volume), typeof(SessionFuelConsumptionControl));
        private static readonly DependencyProperty AverageConsumptionProperty = DependencyProperty.Register("AverageConsumption", typeof(FuelPerDistance), typeof(SessionFuelConsumptionControl));
        private static readonly DependencyProperty VolumeUnitsProperty = DependencyProperty.Register("VolumeUnits",typeof(VolumeUnits), typeof(SessionFuelConsumptionControl));
        private static readonly DependencyProperty DistanceUnitsProperty = DependencyProperty.Register("DistanceUnits", typeof(DistanceUnits), typeof(SessionFuelConsumptionControl));

        public VolumeUnits VolumeUnits
        {
            get => (VolumeUnits) GetValue(VolumeUnitsProperty);
            set => SetValue(VolumeUnitsProperty, value);
        }

        public DistanceUnits DistanceUnits
        {
            get => (DistanceUnits) GetValue(DistanceUnitsProperty);
            set => SetValue(DistanceUnitsProperty, value);
        }

        public FuelPerDistance AverageConsumption
        {
            get => (FuelPerDistance) GetValue(AverageConsumptionProperty);
            set => SetValue(AverageConsumptionProperty, value);
        }

        public TimeSpan RunningTime
        {
            get => (TimeSpan) GetValue(RunningTimeProperty);
            set => SetValue(RunningTimeProperty, value);
        }

        public Volume TotalConsumedFuel
        {
            get => (Volume) GetValue(TotalConsumedFuelProperty);
            set => SetValue(TotalConsumedFuelProperty, value);
        }

        public Volume AvgPerMinute
        {
            get => (Volume)GetValue(AvgPerMinuteProperty);
            set => SetValue(AvgPerMinuteProperty, value);
        }


        public Volume AvgPerLap
        {
            get => (Volume)GetValue(AvgPerLapProperty);
            set => SetValue(AvgPerLapProperty, value);
        }


        public string TrackName
        {
            get => (string)GetValue(TrackNameProperty);
            set => SetValue(TrackNameProperty, value);
        }

        public Distance TraveledDistance
        {
            get => (Distance)GetValue(TraveledDistanceProperty);
            set => SetValue(TraveledDistanceProperty, value);
        }

        public string SessionType
        {
            get => (string)GetValue(SessionTypeProperty);
            set => SetValue(SessionTypeProperty, value);
        }       
    }
}