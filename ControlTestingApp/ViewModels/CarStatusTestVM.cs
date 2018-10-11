namespace ControlTestingApp.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.WindowsControls.WPF.CarSettingsControl;
    using SecondMonitor.WindowsControls.WPF.Commands;

    public class CarStatusTestVM : INotifyPropertyChanged
    {
        private double _tyreCondition = 50.0;
        private TemperatureUnits _temperatureUnits;
        private PressureUnits _pressureUnits = PressureUnits.Atmosphere;

        public CarStatusTestVM()
        {
            TyreCoreRawTemperature = 50;
            BrakeRawTemperature = 200;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public double TyreCondition
        {
            get => _tyreCondition;
            set
            {
                _tyreCondition = value;
                NotifyPropertyChanged();
            }
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

        public IReadOnlyCollection<ITyreSettingsViewModel> Tyres => new List<ITyreSettingsViewModel>() { new TestTyreViewMode("Compound 1"), new TestTyreViewMode("Compound 2"), new TestTyreViewMode("Compound 3") };

        public ICommand ChangeTemperatureUnitsCommand => new RelayCommand(ChangeTemperatureUnits);

        public ICommand ChangePressureUnitsCommand => new RelayCommand(ChangePressureUnits);

        public Temperature IdealBrakeTemperature { get; private set; }

        public Temperature BrakeTemperatureWindow { get; private set; }

        public double TyreCoreRawTemperature
        {
            set
            {
                IdealBrakeTemperature = Temperature.FromCelsius(value);
                NotifyPropertyChanged(nameof(IdealBrakeTemperature));
            }
        }

        public double BrakeRawTemperature
        {
            set
            {
                BrakeTemperatureWindow = Temperature.FromCelsius(value);
                NotifyPropertyChanged(nameof(BrakeTemperatureWindow));
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

        private class TestTyreViewMode : ITyreSettingsViewModel
        {
            public TestTyreViewMode(string name)
            {
                CompoundName = name;
            }

            public string CompoundName { get; set; }
        }
    }
}