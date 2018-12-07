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
        private double _wheelRotation;

        public double ThrottlePercentage
        {
            get => _throttlePercentage;
            set
            {
                _throttlePercentage = value;
                NotifyPropertyChanged();
            }
        }

        public double WheelRotation
        {
            get => _wheelRotation;
            set
            {
                _wheelRotation = value;
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
            if (dataSet?.PlayerInfo?.CarInfo == null || dataSet.InputInfo == null)
            {
                return;
            }

            if (dataSet.InputInfo.ThrottlePedalPosition >= 0)
            {
                _throttlePercentage = dataSet.InputInfo.ThrottlePedalPosition * 100;
            }

            if (dataSet.InputInfo.BrakePedalPosition >= 0)
            {
                _brakePercentage = dataSet.InputInfo.BrakePedalPosition * 100;
            }

            if (dataSet.InputInfo.ClutchPedalPosition >= 0)
            {
                _clutchPercentage = dataSet.InputInfo.ClutchPedalPosition * 100;
            }

            _wheelRotation = dataSet.InputInfo.WheelAngle;
            _gear = dataSet.PlayerInfo.CarInfo.CurrentGear;
            NotifyPropertyChanged(string.Empty);
        }

        public void Reset()
        {

        }
    }
}