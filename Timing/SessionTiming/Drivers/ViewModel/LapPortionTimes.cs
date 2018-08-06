namespace SecondMonitor.Timing.SessionTiming.Drivers.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class LapPortionTimes : INotifyPropertyChanged
    {
        private readonly int _baseTrackPortionLength;

        private readonly TimeSpan[] _trackPortions;

        private int _lastTrackedPortion;

        public event PropertyChangedEventHandler PropertyChanged;

        public LapPortionTimes(int baseTrackPortionLength, double trackLength, LapInfo lap)
        {
            _lastTrackedPortion = -1;
            _baseTrackPortionLength = baseTrackPortionLength;
            Lap = lap;
            _trackPortions = new TimeSpan[(int)trackLength / baseTrackPortionLength + 1];
        }

        public LapInfo Lap { get; }

        public int LastTrackedPortion
        {
            get => _lastTrackedPortion;
            set
            {
                _lastTrackedPortion = value;
                OnPropertyChanged();
            }
        }

        public void UpdateLapPortions()
        {
            int currentPortion = GetIndexByDistance(Lap.CompletedDistance);
            if (currentPortion <= _lastTrackedPortion || currentPortion >= _trackPortions.Length)
            {
                return;
            }

            while (_lastTrackedPortion < currentPortion)
            {
                _lastTrackedPortion++;
                _trackPortions[_lastTrackedPortion] = Lap.CurrentlyValidProgressTime;
            }

            LastTrackedPortion = currentPortion;
        }

        public TimeSpan GetTimeAtDistance(double distance)
        {
            int index = GetIndexByDistance(distance);
            return index > _lastTrackedPortion ? TimeSpan.Zero : _trackPortions[index];
        }

        private int GetIndexByDistance(double trackDistance) => (int)trackDistance / _baseTrackPortionLength;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}