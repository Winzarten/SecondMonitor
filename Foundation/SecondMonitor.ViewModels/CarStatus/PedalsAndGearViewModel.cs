namespace SecondMonitor.ViewModels.CarStatus
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using Properties;

    public class PedalsAndGearViewModel : INotifyPropertyChanged, ISimulatorDataSetViewModel
    {
        private readonly Stopwatch _refreshWatch;
        public event PropertyChangedEventHandler PropertyChanged;

        public double ThrottlePercentage { get; set; }

        public Velocity Speed { get; private set; }

        public int Rpm { get; private set; }

        public double WheelRotation { get; set; }

        public double ClutchPercentage { get; set; }

        public double BrakePercentage { get; set; }

        public string Gear { get; set; }

        public PedalsAndGearViewModel()
        {
            _refreshWatch = Stopwatch.StartNew();
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

            if (_refreshWatch.ElapsedMilliseconds < 50)
            {
                return;
            }

            if (dataSet.InputInfo.ThrottlePedalPosition >= 0)
            {
                ThrottlePercentage = dataSet.InputInfo.ThrottlePedalPosition * 100;
            }

            if (dataSet.InputInfo.BrakePedalPosition >= 0)
            {
                BrakePercentage = dataSet.InputInfo.BrakePedalPosition * 100;
            }

            if (dataSet.InputInfo.ClutchPedalPosition >= 0)
            {
                ClutchPercentage = dataSet.InputInfo.ClutchPedalPosition * 100;
            }

            Speed = dataSet.PlayerInfo.Speed;
            Rpm = dataSet.PlayerInfo.CarInfo.EngineRpm;


            WheelRotation = dataSet.InputInfo.WheelAngle;
            Gear = dataSet.PlayerInfo.CarInfo.CurrentGear;
            NotifyPropertyChanged(string.Empty);
            _refreshWatch.Restart();
        }

        public void Reset()
        {

        }
    }
}