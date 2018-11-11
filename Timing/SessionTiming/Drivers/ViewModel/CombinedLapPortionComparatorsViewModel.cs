namespace SecondMonitor.Timing.SessionTiming.Drivers.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Annotations;

    public class CombinedLapPortionComparatorsViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly DriverTiming _driver;
        private LapPortionTimesComparatorViewModel _playerLapToPrevious;
        private LapPortionTimesComparatorViewModel _playerLapToPlayerBest;

        public CombinedLapPortionComparatorsViewModel(DriverTiming driver)
        {
            _driver = driver;
            _driver.NewLapStarted += DriverOnLapCompletedOrInvalidated;
            _driver.LapCompleted += DriverOnLapCompletedOrInvalidated;
            _driver.LapInvalidated += DriverOnLapCompletedOrInvalidated;
            RecreatePlayerLapToPlayerBest();
        }

        private void DriverOnLapCompletedOrInvalidated(object sender, LapEventArgs e)
        {
            RecreatePlayerLapToPlayerBest();
            RecreatePlayerLapToPrevious();
        }

        public LapPortionTimesComparatorViewModel PlayerLapToPreviousComparator
        {
            get => _playerLapToPrevious;
            protected set
            {
                _playerLapToPrevious = value;
                OnPropertyChanged();
            }
        }

        public LapPortionTimesComparatorViewModel PlayerLapToBestPlayerComparator
        {
            get => _playerLapToPlayerBest;
            protected set
            {
                _playerLapToPlayerBest = value;
                OnPropertyChanged();
            }
        }

        private void RecreatePlayerLapToPrevious()
        {
            _playerLapToPrevious?.Dispose();

            if (_driver.LastCompletedLap == null || _driver.CurrentLap == null)
            {
                return;
            }

            PlayerLapToPreviousComparator = new LapPortionTimesComparatorViewModel(_driver.LastCompletedLap, _driver.CurrentLap);
        }

        private void RecreatePlayerLapToPlayerBest()
        {
            _playerLapToPlayerBest?.Dispose();

            if (_driver.BestLap == null || _driver.CurrentLap == null)
            {
                return;
            }

            PlayerLapToBestPlayerComparator = new LapPortionTimesComparatorViewModel(_driver.BestLap, _driver.CurrentLap);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _playerLapToPrevious?.Dispose();
            _playerLapToPlayerBest?.Dispose();
            _driver.NewLapStarted -= DriverOnLapCompletedOrInvalidated;
            _driver.LapCompleted -= DriverOnLapCompletedOrInvalidated;
            _driver.LapInvalidated -= DriverOnLapCompletedOrInvalidated;
        }
    }
}