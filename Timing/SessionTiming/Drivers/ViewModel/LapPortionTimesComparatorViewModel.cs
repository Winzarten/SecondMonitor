namespace SecondMonitor.Timing.SessionTiming.Drivers.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class LapPortionTimesComparatorViewModel : INotifyPropertyChanged,  IDisposable
    {
        private TimeSpan _timeDifference;

        private DateTime _nextUpdate;

        public LapPortionTimesComparatorViewModel(LapInfo referenceLap, LapInfo comparedLap)
        {
            _nextUpdate = DateTime.Now;
            ReferenceLap = referenceLap;
            ComparedLap = comparedLap;
            TimeDifference = TimeSpan.Zero;
            SubscribeToComparedLap();
        }

        public LapInfo ReferenceLap { get; }
        public LapInfo ComparedLap { get; }

        public TimeSpan TimeDifference
        {
            get => _timeDifference;
            set
            {
                _timeDifference = value;
                OnPropertyChanged();
            }
        }


        private void SubscribeToComparedLap()
        {
            ComparedLap.LapTelemetryInfo.PortionTimes.PropertyChanged += PortionTimes_PropertyChanged;
        }

        private void UnSubscribeToComparedLap()
        {
            if (ComparedLap?.LapTelemetryInfo?.PortionTimes == null)
            {
                return;
            }

            ComparedLap.LapTelemetryInfo.PortionTimes.PropertyChanged -= PortionTimes_PropertyChanged;
        }

        private void PortionTimes_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (DateTime.Now < _nextUpdate)
            {
                return;
            }

            if (ReferenceLap.LapTelemetryInfo.PortionTimes.GetTimeAtDistance(ComparedLap.CompletedDistance) != TimeSpan.Zero)
            {
                TimeDifference = ComparedLap.LapTelemetryInfo.PortionTimes.GetTimeAtDistance(ComparedLap.CompletedDistance)
                                 - ReferenceLap.LapTelemetryInfo.PortionTimes.GetTimeAtDistance(ComparedLap.CompletedDistance);
            }
            else
            {
                TimeDifference = TimeSpan.Zero;
            }
            _nextUpdate = DateTime.Now + TimeSpan.FromMilliseconds(100);
        }

        public void Dispose()
        {
            UnSubscribeToComparedLap();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}