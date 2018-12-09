namespace SecondMonitor.Timing.SessionTiming.Drivers.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Annotations;

    public class CombinedLapPortionComparatorsViewModel : INotifyPropertyChanged, IDisposable
    {
        private  DriverTiming _driver;
        private LapPortionTimesComparatorViewModel _playerLapToPrevious;
        private LapPortionTimesComparatorViewModel _playerLapToPlayerBest;

        public CombinedLapPortionComparatorsViewModel(DriverTiming driver)
        {
            Driver = driver;
            _driver.NewLapStarted += DriverOnLapCompletedOrInvalidated;
            _driver.LapCompleted += DriverOnLapCompletedOrInvalidated;
            _driver.LapInvalidated += DriverOnLapCompletedOrInvalidated;
            PlayerLapToBestPlayerComparator = new LapPortionTimesComparatorViewModel();
            PlayerLapToPreviousComparator = new LapPortionTimesComparatorViewModel();
            RecreatePlayerLapToPlayerBest();
        }

        public DriverTiming Driver
        {
            get => _driver;
            set
            {
                UnSubscribeToDriverEvents();
                _driver = value;
                SubscribeToDriverEvents();
            }
        }

        private void SubscribeToDriverEvents()
        {
            if (_driver == null)
            {
                return;
            }

            _driver.NewLapStarted += DriverOnLapCompletedOrInvalidated;
            _driver.LapCompleted += DriverOnLapCompletedOrInvalidated;
            _driver.LapInvalidated += DriverOnLapCompletedOrInvalidated;
        }

        private void UnSubscribeToDriverEvents()
        {
            if (_driver == null)
            {
                return;
            }

            _driver.NewLapStarted -= DriverOnLapCompletedOrInvalidated;
            _driver.LapCompleted -= DriverOnLapCompletedOrInvalidated;
            _driver.LapInvalidated -= DriverOnLapCompletedOrInvalidated;
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

            PlayerLapToPreviousComparator.ChangeReferencedLaps(_driver.LastCompletedLap, _driver.CurrentLap);
        }

        private void RecreatePlayerLapToPlayerBest()
        {
            _playerLapToPlayerBest?.Dispose();

            if (_driver.BestLap == null || _driver.CurrentLap == null)
            {
                return;
            }

            PlayerLapToBestPlayerComparator.ChangeReferencedLaps(_driver.BestLap, _driver.CurrentLap);
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