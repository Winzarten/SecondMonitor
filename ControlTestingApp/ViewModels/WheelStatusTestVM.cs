namespace ControlTestingApp.ViewModels
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.WindowsControls.WPF.Commands;

    public class WheelStatusTestVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double _tyreCondition = 50.0;
        private TemperatureUnits _temperatureUnits;

        public double TyreCondition
        {
            get => _tyreCondition;
            set
            {
                _tyreCondition = value;
                NotifyPropertyChanged();
            }
        }

        public WheelStatusTestVM()
        {
            TyreCoreRawTemperature = 50;
            BrakeRawTemperature = 200;
        }

        public TemperatureUnits TemperatureUnits
        {
            get => _temperatureUnits;
            set
            {
                _temperatureUnits = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand ChangeTemperatureUnitsCommand => new RelayCommand(ChangeTemperatureUnits);



        public OptimalQuantity<Temperature> TyreCoreTemperature { get; private set; }

        public OptimalQuantity<Temperature> BrakeTemperature { get; private set; }

        public double TyreCoreRawTemperature
        {
            set
            {
                TyreCoreTemperature = new OptimalQuantity<Temperature>()
                                          {
                                              ActualQuantity =
                                                  Temperature.FromCelsius(value),
                                              IdealQuantity =
                                                  Temperature.FromCelsius(80),
                                              IdealQuantityWindow =
                                                  Temperature.FromCelsius(50)
                                          };
                NotifyPropertyChanged(nameof(TyreCoreTemperature));
            }
        }

        public double BrakeRawTemperature
        {
            set
            {
                BrakeTemperature = new OptimalQuantity<Temperature>()
                                          {
                                              ActualQuantity =
                                                  Temperature.FromCelsius(value),
                                              IdealQuantity =
                                                  Temperature.FromCelsius(350),
                                              IdealQuantityWindow =
                                                  Temperature.FromCelsius(150)
                                          };
                NotifyPropertyChanged(nameof(BrakeTemperature));
            }
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ChangeTemperatureUnits()
        {
            if (TemperatureUnits == TemperatureUnits.Celsius)
            {
                TemperatureUnits = TemperatureUnits.Kelvin;
                return;
            }

            if (TemperatureUnits == TemperatureUnits.Kelvin)
            {
                TemperatureUnits = TemperatureUnits.Fahrenheit;
                return;
            }

            if (TemperatureUnits == TemperatureUnits.Fahrenheit)
            {
                TemperatureUnits = TemperatureUnits.Celsius;
                return;
            }

        }
    }
}