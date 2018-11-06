namespace SecondMonitor.ViewModels.CarStatus
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using DataModel.Snapshot;
    using Properties;

    public class PedalsAndGearViewModel : INotifyPropertyChanged, ISimulatorDataSetViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double _throttlePercentage;
        private double _clutchPercentage;
        private double _brakePercentage;
        private string _gear;

        public double ThrottlePercentage
        {
            get => _throttlePercentage;
            set
            {
                _throttlePercentage = value;
                NotifyPropertyChanged();
            }
        }

        public double ClutchPercentage
        {
            get => _clutchPercentage;
            set
            {
                _clutchPercentage = value;
                NotifyPropertyChanged();
            }
        }

        public double BrakePercentage
        {
            get => _brakePercentage;
            set
            {
                _brakePercentage = value;
                NotifyPropertyChanged();
            }
        }

        public string Gear
        {
            get => _gear;
            set
            {
                _gear = value;
                NotifyPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
            if (dataSet?.PlayerInfo?.CarInfo == null || dataSet.PedalInfo == null)
            {
                return;
            }

            if (dataSet.PedalInfo.ThrottlePedalPosition >= 0)
            {
                ThrottlePercentage = dataSet.PedalInfo.ThrottlePedalPosition * 100;
            }

            if (dataSet.PedalInfo.BrakePedalPosition >= 0)
            {
                BrakePercentage = dataSet.PedalInfo.BrakePedalPosition * 100;
            }

            if (dataSet.PedalInfo.ClutchPedalPosition >= 0)
            {
                ClutchPercentage = dataSet.PedalInfo.ClutchPedalPosition * 100;
            }

            Gear = dataSet.PlayerInfo.CarInfo.CurrentGear;
        }

        public void Reset()
        {

        }
    }
}