
namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    using System;
    using System.Windows;
    using Contracts.FuelInformation;
    using DataModel.BasicProperties;

    public class FuelCalculatorViewModel : DependencyObject, IFuelCalculatorViewModel
    {
        private static readonly DependencyProperty RequiredLapsProperty = DependencyProperty.Register("RequiredLaps", typeof(int), typeof(FuelCalculatorViewModel), new PropertyMetadata() {PropertyChangedCallback = OnRequiredPropertyChanged});
        private static readonly DependencyProperty RequiredMinutesProperty = DependencyProperty.Register("RequiredMinutes", typeof(int), typeof(FuelCalculatorViewModel), new PropertyMetadata() { PropertyChangedCallback = OnRequiredPropertyChanged });
        private static readonly DependencyProperty LapDistanceProperty = DependencyProperty.Register("LapDistance", typeof(double), typeof(FuelCalculatorViewModel));
        private static readonly DependencyProperty RequiredFuelProperty = DependencyProperty.Register("RequiredFuel", typeof(Volume), typeof(FuelCalculatorViewModel));
        private static readonly DependencyProperty FuelConsumptionProperty = DependencyProperty.Register("FuelConsumption", typeof(IFuelConsumptionInfo), typeof(FuelCalculatorViewModel));

        public IFuelConsumptionInfo FuelConsumption
        {
            get => (IFuelConsumptionInfo) GetValue(FuelConsumptionProperty);
            set => SetValue(FuelConsumptionProperty, value);
        }

        public int RequiredLaps
        {
            get => (int) GetValue(RequiredLapsProperty);
            set => SetValue(RequiredLapsProperty, value);
        }

        public int RequiredMinutes
        {
            get => (int)GetValue(RequiredMinutesProperty);
            set => SetValue(RequiredMinutesProperty, value);
        }

        public double LapDistance
        {
            get => (double)GetValue(LapDistanceProperty);
            set => SetValue(LapDistanceProperty, value);
        }

        public Volume RequiredFuel
        {
            get => (Volume) GetValue(RequiredFuelProperty);
            set => SetValue(RequiredFuelProperty, value);
        }

        private void CalculateRequiredFuel()
        {
            RequiredFuelCalculator calculator = new RequiredFuelCalculator(FuelConsumption);
            RequiredFuel = calculator.GetRequiredFuel(TimeSpan.FromMinutes(RequiredMinutes),
                Distance.FromMeters(LapDistance * RequiredLaps));
        }

        private static void OnRequiredPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FuelCalculatorViewModel fuelCalculatorViewModel)
            {
                fuelCalculatorViewModel.CalculateRequiredFuel();
            }
        }
    }


}
