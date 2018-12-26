namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.Replay
{
    using System;
    using System.Windows.Input;
    using DataModel.BasicProperties;
    using SecondMonitor.ViewModels;
    using Settings;

    public class ReplayViewModel : AbstractViewModel, IReplayViewModel
    {
        private Distance _trackLength;
        private double _selectedDistance;
        private DistanceUnits _distanceUnits;
        private bool _isEnabled;
        private Distance _displayDistance;
        private TimeSpan _displayTime;
        private ICommand _stopCommand;
        private ICommand _playCommand;
        private ICommand _nextFrameCommand;
        private ICommand _previousFrameCommand;
        private ICommand _syncDistancesCommand;

        public double TrackLengthRaw => TrackLength?.GetByUnit(DistanceUnits)?? 0;

        public Distance TrackLength
        {
            get => _trackLength;
            set
            {
                _trackLength = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(TrackLengthRaw));
            }
        }

        public Distance DisplayDistance
        {
            get => _displayDistance;
            set
            {
                _displayDistance = value;
                NotifyPropertyChanged();
            }
        }
        public TimeSpan DisplayTime
        {
            get => _displayTime;
            set
            {
                _displayTime = value;
                NotifyPropertyChanged();
            }
        }

        public DistanceUnits DistanceUnits
        {
            get => _distanceUnits;
            set
            {
                _distanceUnits = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(TrackLengthRaw));
            }
        }

        public double SelectedDistance
        {
            get => _selectedDistance;
            set
            {
                _selectedDistance = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand StopCommand
        {
            get => _stopCommand;
            set
            {
                _stopCommand = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand PlayCommand
        {
            get => _playCommand;
            set
            {
                _playCommand = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand NextFrameCommand
        {
            get => _nextFrameCommand;
            set
            {
                _nextFrameCommand = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand PreviousFrameCommand
        {
            get => _previousFrameCommand;
            set
            {
                _previousFrameCommand = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand SyncDistancesCommand
        {
            get => _syncDistancesCommand;
            set
            {
                _syncDistancesCommand = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged();
            }
        }
    }
}