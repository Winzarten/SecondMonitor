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
        private PressureUnits _pressureUnits = PressureUnits.Atmosphere;

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

        public PressureUnits PressureUnits
        {
            get => _pressureUnits;
            set
            {
                _pressureUnits = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand ChangeTemperatureUnitsCommand => new RelayCommand(ChangeTemperatureUnits);

        public ICommand ChangePressureUnitsCommand => new RelayCommand(ChangePressureUnits);



        public OptimalQuantity<Temperature> TyreCoreTemperature { get; private set; }

        public OptimalQuantity<Temperature> TyreLeftTemperature { get; private set; }

        public OptimalQuantity<Temperature> TyreRightTemperature { get; private set; }

        public OptimalQuantity<Temperature> TyreCenterTemperature { get; private set; }

        public OptimalQuantity<Temperature> BrakeTemperature { get; private set; }

        public OptimalQuantity<Pressure> TyrePressure { get; private set; }

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
                                                  Temperature.FromCelsius(15)
                                          };
                NotifyPropertyChanged(nameof(TyreCoreTemperature));
            }
        }

        public double TyreLeftRawTemperature
        {
            set
            {
                TyreLeftTemperature = new OptimalQuantity<Temperature>()
                                          {
                                              ActualQuantity =
                                                  Temperature.FromCelsius(value),
                                              IdealQuantity =
                                                  Temperature.FromCelsius(80),
                                              IdealQuantityWindow =
                                                  Temperature.FromCelsius(15)
                                          };
                NotifyPropertyChanged(nameof(TyreLeftTemperature));
            }
        }

        public double TyreCenterRawTemperature
        {
            set
            {
                TyreCenterTemperature = new OptimalQuantity<Temperature>()
                                          {
                                              ActualQuantity =
                                                  Temperature.FromCelsius(value),
                                              IdealQuantity =
                                                  Temperature.FromCelsius(80),
                                              IdealQuantityWindow =
                                                  Temperature.FromCelsius(15)
                                          };
                NotifyPropertyChanged(nameof(TyreCenterTemperature));
            }
        }

        public double TyreRightRawTemperature
        {
            set
            {
                TyreRightTemperature = new OptimalQuantity<Temperature>()
                                          {
                                              ActualQuantity =
                                                  Temperature.FromCelsius(value),
                                              IdealQuantity =
                                                  Temperature.FromCelsius(80),
                                              IdealQuantityWindow =
                                                  Temperature.FromCelsius(15)
                                          };
                NotifyPropertyChanged(nameof(TyreRightTemperature));
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
                                                  Temperature.FromCelsius(100)
                                          };
                NotifyPropertyChanged(nameof(BrakeTemperature));
            }
        }

        public double TyrePressureRaw
        {
            set
            {
                TyrePressure = new OptimalQuantity<Pressure>()
                                       {
                                           ActualQuantity =
                                               Pressure.FromKiloPascals(value),
                                           IdealQuantity =
                                               Pressure.FromKiloPascals(190),
                                           IdealQuantityWindow =
                                               Pressure.FromKiloPascals(20)
                                       };
                NotifyPropertyChanged(nameof(TyrePressure));
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

        private void ChangePressureUnits()
        {
            switch (PressureUnits)
            {
                case PressureUnits.Kpa:
                    PressureUnits = PressureUnits.Atmosphere;
                    break;
                case PressureUnits.Atmosphere:
                    PressureUnits = PressureUnits.Bar;
                    break;
                case PressureUnits.Bar:
                    PressureUnits = PressureUnits.Psi;
                    break;
                case PressureUnits.Psi:
                    PressureUnits = PressureUnits.Kpa;
                    break;
                default:
                    break;
            }
        }
    }
}