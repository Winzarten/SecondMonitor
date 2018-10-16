namespace SecondMonitor.ViewModels.Base
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using DataModel.BasicProperties;
    using DataModel.Snapshot;

    public abstract class AbstractTemperatureViewModel : DependencyObject, INotifyPropertyChanged, ISimulatorDataSetViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Temperature _temperature;

        public Temperature Temperature
        {
            get => _temperature;
            set
            {
                if (_temperature == value)
                {
                    return;
                }

                _temperature = value;
                NotifyPropertyChanged();
            }
        }

        public abstract Temperature MinimalTemperature
        {
            get;
            protected set;
        }

        public abstract Temperature MaximumTemperature
        {
            get;
            protected set;
        }

        public abstract Temperature MaximumNormalTemperature
        {
            get;
            protected set;
        }

        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
            Temperature newTemperature = GetTemperatureFromDataSet(dataSet);
            if (Temperature == null || Math.Abs(newTemperature.InCelsius - Temperature.InCelsius) > 0.1)
            {
                Temperature = newTemperature;
            }
        }

        public void Reset()
        {
            // Nothing to do
        }

        protected abstract Temperature GetTemperatureFromDataSet(SimulatorDataSet dataSet);

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}