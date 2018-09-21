namespace SecondMonitor.Timing.SessionTiming.Drivers.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using SecondMonitor.Timing.Annotations;

    public class CombinedLapPortionComparatorsVM : INotifyPropertyChanged, IDisposable
    {
        private LapPortionTimesComparatorViewModel _playerLapToPrevious;
        private LapPortionTimesComparatorViewModel _playerLapToPlayerBest;

        private LapInfo _playerLap;

        public CombinedLapPortionComparatorsVM(LapInfo playerLap)
        {
            _playerLap = playerLap;
            if (playerLap?.Driver != null)
            {
                _playerLap.Driver.LapCompleted += DriverOnLapCompleted;
            }
            RecreatePlayerLapToPrevious();
            RecreatePlayerLapToPlayerBest();
        }

        private void DriverOnLapCompleted(object sender, LapEventArgs e)
        {
            if (e.Lap == _playerLap.Driver.BestLap)
            {
                RecreatePlayerLapToPlayerBest();
            }
        }

        public LapInfo PlayerLap
        {
            get => _playerLap;
            set
            {
                if (_playerLap != null)
                {
                    _playerLap.Driver.LapCompleted -= DriverOnLapCompleted;
                }

                _playerLap = value;
                OnPropertyChanged();
                RecreatePlayerLapToPrevious();
                RecreatePlayerLapToPlayerBest();
            }
        }

        public LapPortionTimesComparatorViewModel PlayerLapToPreviousComparator
        {
            get => _playerLapToPrevious;
            set
            {
                _playerLapToPrevious = value;
                OnPropertyChanged();
            }
        }

        public LapPortionTimesComparatorViewModel PlayerLapToBestPlayerComparator
        {
            get => _playerLapToPlayerBest;
            set
            {
                _playerLapToPlayerBest = value;
                OnPropertyChanged();
            }
        }

        private void RecreatePlayerLapToPrevious()
        {
            _playerLapToPrevious?.Dispose();

            if (_playerLap?.Driver.LastCompletedLap == null)
            {
                return;
            }

            PlayerLapToPreviousComparator = new LapPortionTimesComparatorViewModel(_playerLap.Driver.LastCompletedLap, _playerLap);
        }

        private void RecreatePlayerLapToPlayerBest()
        {
            _playerLapToPlayerBest?.Dispose();

            if (_playerLap?.Driver.BestLap == null)
            {
                return;
            }

            PlayerLapToBestPlayerComparator = new LapPortionTimesComparatorViewModel(_playerLap.Driver.BestLap, _playerLap);
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
            _playerLap.Driver.LapCompleted -= DriverOnLapCompleted;
        }
    }
}