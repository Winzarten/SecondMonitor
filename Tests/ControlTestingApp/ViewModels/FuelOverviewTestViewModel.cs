namespace ControlTestingApp.ViewModels
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Annotations;

    using SecondMonitor.DataModel.BasicProperties;

    public class FuelOverviewTestViewModel : INotifyPropertyChanged
    {
        public FuelLevelStatus FuelLevelStatus
        {
            get;
            private set;
        }

        public Volume FuelLeft
        {
            get;
            private set;
        }

        public double FuelLeftRaw
        {
            set
            {
                FuelLeft = Volume.FromLiters(value);
                OnPropertyChanged(nameof(FuelLeft));
            }
        }

        public double FuelStatusDouble
        {
            set
            {
                int fuelState = (int)value;
                FuelLevelStatus = (FuelLevelStatus)fuelState;
                OnPropertyChanged(nameof(FuelLevelStatus));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}