namespace SecondMonitor.ViewModels.Base
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;

    public abstract class AbstractTemperatureViewModel : DependencyObject, INotifyPropertyChanged, ISimulatorDataSetViewModel, IViewModelWithIcon
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

        public abstract ImageSource Icon
        {
            get;
            protected set;
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
            Temperature = GetTemperatureFromDataSet(dataSet);
        }

        protected abstract Temperature GetTemperatureFromDataSet(SimulatorDataSet dataSet);

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}