namespace SecondMonitor.ViewModels.CarStatus
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;
    using DataModel.Snapshot.Systems;
    using Properties;

    public class CarWheelsViewModel : DependencyObject, ISimulatorDataSetViewModel, INotifyPropertyChanged
    {
        private WheelStatusViewModel _leftFrontTyre;
        private WheelStatusViewModel _rightFrontTyre;
        private WheelStatusViewModel _leftRearTyre;
        private WheelStatusViewModel _rightRearTyre;

        public CarWheelsViewModel()
        {
            LeftFrontTyre = new WheelStatusViewModel(true);
            LeftRearTyre = new WheelStatusViewModel(true);
            RightFrontTyre = new WheelStatusViewModel(false);
            RightRearTyre = new WheelStatusViewModel(false);
        }

        public WheelStatusViewModel LeftFrontTyre
        {
            get => _leftFrontTyre;
            private set
            {
                _leftFrontTyre = value;
                NotifyPropertyChanged();
            }
        }

        public WheelStatusViewModel RightFrontTyre
        {
            get => _rightFrontTyre;
            private set
            {
                _rightFrontTyre = value;
                NotifyPropertyChanged();
            }
        }

        public WheelStatusViewModel LeftRearTyre
        {
            get => _leftRearTyre;
            private set
            {
                _leftRearTyre = value;
                NotifyPropertyChanged();
            }
        }

        public WheelStatusViewModel RightRearTyre
        {
            get => _rightRearTyre;
            private set
            {
                _rightRearTyre = value;
                NotifyPropertyChanged();
            }
        }


        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
            if (dataSet?.PlayerInfo?.CarInfo?.WheelsInfo == null)
            {
                return;
            }

            ApplyPlayerInfo(dataSet.PlayerInfo);
        }

        public void ApplyPlayerInfo(DriverInfo playerInfo)
        {
            Wheels wheels = playerInfo?.CarInfo?.WheelsInfo;

            if (wheels == null)
            {
                return;
            }

            LeftFrontTyre.ApplyWheelCondition(wheels.FrontLeft);
            RightFrontTyre.ApplyWheelCondition(wheels.FrontRight);
            LeftRearTyre.ApplyWheelCondition(wheels.RearLeft);
            RightRearTyre.ApplyWheelCondition(wheels.RearRight);
        }

        public void Reset()
        {
            // Nothing to do
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}