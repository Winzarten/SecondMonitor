namespace SecondMonitor.Timing.SessionTiming.Drivers.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class LapPortionTimesComparatorViewModel : INotifyPropertyChanged,  IDisposable
    {
        private TimeSpan _timeDifference;

        public LapPortionTimesComparatorViewModel(LapInfo referenceLap, LapInfo comparedLap)
        {
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
            ComparedLap.LapTelemetryInfo.PortionTimes.PropertyChanged -= PortionTimes_PropertyChanged;
        }

        private void PortionTimes_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ReferenceLap.LapTelemetryInfo.PortionTimes.GetTimeAtDistance(ComparedLap.CompletedDistance) != TimeSpan.Zero)
            {
                TimeDifference = ComparedLap.LapTelemetryInfo.PortionTimes.GetTimeAtDistance(ComparedLap.CompletedDistance)
                                 - ReferenceLap.LapTelemetryInfo.PortionTimes.GetTimeAtDistance(ComparedLap.CompletedDistance);
            }
            else
            {
                TimeDifference = TimeSpan.Zero;
            }
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