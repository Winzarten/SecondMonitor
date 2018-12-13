namespace SecondMonitor.Timing.SessionTiming.Drivers.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class LapPortionTimesComparatorViewModel : INotifyPropertyChanged,  IDisposable
    {
        private TimeSpan _timeDifference;

        private DateTime _nextUpdate;

        public LapPortionTimesComparatorViewModel()
        {
            _nextUpdate = DateTime.Now;
            TimeDifference = TimeSpan.Zero;
            SubscribeToComparedLap();
        }

        public LapInfo ReferenceLap { get; private set; }
        public LapInfo ComparedLap { get; private set; }

        public TimeSpan TimeDifference
        {
            get => _timeDifference;
            set
            {
                _timeDifference = value;
                OnPropertyChanged();
            }
        }

        public void ChangeReferencedLaps(LapInfo referencedLap, LapInfo comparedLap)
        {
            UnSubscribeToComparedLap();
            ReferenceLap = referencedLap;
            ComparedLap = comparedLap;
            SubscribeToComparedLap();
        }


        private void SubscribeToComparedLap()
        {
            if (ComparedLap?.LapTelemetryInfo?.PortionTimes == null)
            {
                return;
            }
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

        private void PortionTimes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (DateTime.Now < _nextUpdate)
            {
                return;
            }

            if (ReferenceLap?.LapTelemetryInfo?.PortionTimes == null || ReferenceLap.LapTelemetryInfo.IsPurged)
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