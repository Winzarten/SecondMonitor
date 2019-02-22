namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using Contracts.Commands;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Systems;
    using Properties;

    public class FuelOverviewViewModel : ISimulatorDataSetViewModel, INotifyPropertyChanged
    {
        private ICommand _resetCommand;
        private TimeSpan _timeLeft;
        private double _lapsLeft;
        private Volume _avgPerLap;
        private Volume _fuelLeft;
        private Volume _avgPerMinute;
        private Volume _currentPerLap;
        private Volume _currentPerMinute;
        private double _fuelPercentage;
        private FuelLevelStatus _fuelLevelState;
        private Volume _maximumFuel;
        private bool _showDeltaInfo;
        private TimeSpan _timeDelta;
        private double _lapsDelta;
        private Volume _fuelDelta;

        private readonly FuelConsumptionMonitor _fuelConsumptionMonitor;
        private readonly SessionRemainingCalculator _sessionRemainingCalculator;

        public FuelOverviewViewModel(IPaceProvider paceProvider)
        {
            _sessionRemainingCalculator = new SessionRemainingCalculator(paceProvider);
            _fuelConsumptionMonitor = new FuelConsumptionMonitor();
            _resetCommand = new RelayCommand(Reset);
        }

        public FuelConsumptionMonitor FuelConsumptionMonitor => _fuelConsumptionMonitor;

        public ICommand ResetCommand
        {
            get => _resetCommand;
            private set
            {
                _resetCommand = value;
                NotifyPropertyChanged();
            }
        }

        public TimeSpan TimeDelta
        {
            get => _timeDelta;
            set
            {
                _timeDelta = value;
                NotifyPropertyChanged();
            }
        }

        public double LapsDelta
        {
            get => _lapsDelta;
            set
            {
                _lapsDelta = value;
                NotifyPropertyChanged();
            }
        }

        public Volume FuelDelta
        {
            get => _fuelDelta;
            set
            {
                _fuelDelta = value;
                NotifyPropertyChanged();
            }
        }

        public bool ShowDeltaInfo
        {
            get => _showDeltaInfo;
            set
            {
                if (value == _showDeltaInfo)
                {
                    return;
                }

                _showDeltaInfo = value;
                NotifyPropertyChanged();
            }
        }

        public TimeSpan TimeLeft
        {
            get => _timeLeft;
            private set
            {
                _timeLeft = value;
                NotifyPropertyChanged();
            }
        }

        public double LapsLeft
        {
            get => _lapsLeft;
            set
            {
                _lapsLeft = value;
                NotifyPropertyChanged();
            }
        }

        public Volume AvgPerLap
        {
            get => _avgPerLap;
            set
            {
                _avgPerLap = value;
                NotifyPropertyChanged();
            }
        }

        public Volume AvgPerMinute
        {
            get => _avgPerMinute;
            set
            {
                _avgPerMinute = value;
                NotifyPropertyChanged();
            }
        }

        public Volume CurrentPerLap
        {
            get => _currentPerLap;
            set
            {
                _currentPerLap = value;
                NotifyPropertyChanged();
            }
        }

        public Volume CurrentPerMinute
        {
            get => _currentPerMinute;
            set
            {
                _currentPerMinute = value;
                NotifyPropertyChanged();
            }
        }

        public double FuelPercentage
        {
            get => _fuelPercentage;
            set
            {
                _fuelPercentage = value;
                NotifyPropertyChanged();
            }
        }

        public FuelLevelStatus FuelState
        {
            get => _fuelLevelState;
            set
            {
                if (_fuelLevelState == value)
                {
                    return;
                }

                _fuelLevelState = value;
                NotifyPropertyChanged();
            }
        }

        public Volume MaximumFuel
        {
            get => _maximumFuel;
            private set
            {
                _maximumFuel = value;
                NotifyPropertyChanged();
            }
        }

        private void ReApplyFuelLevels(FuelInfo fuel)
        {
            if (MaximumFuel != fuel.FuelCapacity)
            {
                MaximumFuel = fuel.FuelCapacity;
            }

            _fuelLeft = fuel.FuelRemaining;
            double fuelPercentage = (fuel.FuelRemaining.InLiters / MaximumFuel.InLiters) * 100;
            FuelPercentage = double.IsNaN(fuelPercentage) || double.IsInfinity(fuelPercentage) ? 0 : fuelPercentage;
        }

        private void UpdateActualData(SimulatorDataSet dataSet)
        {
            CurrentPerLap = _fuelConsumptionMonitor.ActPerLap;
            CurrentPerMinute = _fuelConsumptionMonitor.ActPerMinute;
            AvgPerLap = _fuelConsumptionMonitor.TotalPerLap;
            AvgPerMinute = _fuelConsumptionMonitor.TotalPerMinute;

            if (AvgPerLap.InLiters > 0 && AvgPerMinute.InLiters > 0)
            {
                LapsLeft = _fuelLeft.InLiters / AvgPerLap.InLiters;
                TimeLeft = TimeSpan.FromMinutes(_fuelLeft.InLiters / AvgPerMinute.InLiters);
                UpdateFuelState(dataSet);
            }
        }

        private void UpdateFuelState(SimulatorDataSet dataSet)
        {
            switch (dataSet.SessionInfo.SessionType)
            {
                case SessionType.Qualification:
                    FuelState = FuelLevelStatus.Unknown;
                    break;
                case SessionType.Race:
                    UpdateFuelStateByLapsSessionLength(dataSet);
                    break;
                default:
                    UpdateFuelStateByLapsLeft();
                    break;
            }
        }

        private void UpdateFuelStateBySessionLeft(SimulatorDataSet dataSet)
        {
            if (dataSet.PlayerInfo.CompletedLaps == 0)
            {
                FuelState = FuelLevelStatus.Unknown;
                return;
            }

            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Laps)
            {
                UpdateFuelStateByLapsSessionLength(dataSet);
            }
            else
            {
                UpdateFuelStateByToTimeLeft(dataSet);
            }
        }

        private void UpdateFuelStateByToTimeLeft(SimulatorDataSet dataSet)
        {
            TimeDelta = TimeSpan.FromSeconds(TimeLeft.TotalSeconds - dataSet.SessionInfo.SessionTimeRemaining);
            LapsDelta = FuelDelta.InLiters / AvgPerLap.InLiters;
            FuelDelta = Volume.FromLiters(TimeDelta.TotalMinutes * AvgPerMinute.InLiters);
            if (TimeDelta.TotalMinutes > 3)
            {
                FuelState = FuelLevelStatus.IsEnoughForSession;
                return;
            }

            if (TimeDelta.TotalMinutes > 1)
            {
                FuelState = FuelLevelStatus.PossiblyEnoughForSession;
                return;
            }

            if (LapsLeft < 2)
            {
                FuelState = FuelLevelStatus.Critical;
            }
            else
            {
                FuelState = FuelLevelStatus.NotEnoughForSession;
            }
        }

        private void UpdateFuelStateByLapsSessionLength(SimulatorDataSet dataSet)
        {
            if (dataSet.LeaderInfo == null)
            {
                return;
            }

            double lapsToGo = _sessionRemainingCalculator.GetLapsRemaining(dataSet);
            if (double.IsNaN(lapsToGo) || double.IsInfinity(lapsToGo))
            {
                return;
            }
            LapsDelta = LapsLeft - lapsToGo;
            FuelDelta = Volume.FromLiters(CurrentPerLap.InLiters * LapsDelta);
            TimeDelta = TimeSpan.FromMinutes(FuelDelta.InLiters / AvgPerMinute.InLiters);
            if (LapsDelta > 1.5)
            {
                FuelState = FuelLevelStatus.IsEnoughForSession;
                return;
            }

            if (LapsDelta > 0)
            {
                FuelState = FuelLevelStatus.PossiblyEnoughForSession;
                return;
            }

            if (LapsLeft < 2)
            {
                FuelState = FuelLevelStatus.Critical;
            }
            else
            {
                FuelState = FuelLevelStatus.NotEnoughForSession;
            }
        }

        private void UpdateFuelStateByLapsLeft()
        {
            if (LapsLeft < 2)
            {
                FuelState = FuelLevelStatus.Critical;
                return;
            }

            if (LapsLeft < 4)
            {
                FuelState = FuelLevelStatus.NotEnoughForSession;
                return;
            }

            if (LapsLeft < 8)
            {
                FuelState = FuelLevelStatus.PossiblyEnoughForSession;
                return;
            }

            FuelState = FuelLevelStatus.IsEnoughForSession;
        }

        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
            try
            {
                ShowDeltaInfo = dataSet.SessionInfo.SessionType == SessionType.Race;
                ReApplyFuelLevels(dataSet.PlayerInfo.CarInfo.FuelSystemInfo);
                _fuelConsumptionMonitor.UpdateFuelConsumption(dataSet);
                UpdateActualData(dataSet);
            }
            catch (Exception ex)
            {

            }
        }

        public void Reset()
        {
            _fuelConsumptionMonitor.Reset();
            CurrentPerLap = _fuelConsumptionMonitor.ActPerLap;
            CurrentPerMinute = _fuelConsumptionMonitor.ActPerMinute;
            AvgPerLap = _fuelConsumptionMonitor.TotalPerLap;
            AvgPerMinute = _fuelConsumptionMonitor.TotalPerMinute;
            FuelState = FuelLevelStatus.Unknown;
            TimeDelta = TimeSpan.Zero;
            LapsDelta = 0;
            FuelDelta = Volume.FromLiters(0);
            ShowDeltaInfo = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}