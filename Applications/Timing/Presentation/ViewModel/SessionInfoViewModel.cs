﻿namespace SecondMonitor.Timing.Presentation.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;
    using DataModel.BasicProperties;
    using DataModel.Extensions;
    using DataModel.Snapshot;
    using SecondMonitor.Timing.SessionTiming.Drivers.ViewModel;
    using SecondMonitor.Timing.SessionTiming.ViewModel;
    using ViewModels;

    public class SessionInfoViewModel : ISimulatorDataSetViewModel, INotifyPropertyChanged
    {

        private string _bestSector1;
        private string _bestSector2;
        private string _bestSector3;
        private bool _anySectorFilled;
        private string _bestLap;
        private string _sessionRemaining;

        private SessionTiming _timing;

        public SessionInfoViewModel()
        {
            _timing = null;
            BestSector1 = "S1:N/A";
            BestSector2 = "S2:N/A";
            BestSector3 = "S2:N/A";
            BestLap = "Best Lap: N/A";
            SessionRemaining = "L1/14";
            AnySectorFilled = true;
        }

        public SessionInfoViewModel(SessionTiming timing)
        {
            _timing = timing;
        }

        public string BestSector1
        {
            get => _bestSector1;
            set
            {
                _bestSector1 = value;
                OnPropertyChanged();
            }
        }

        public string BestSector2
        {
            get => _bestSector2;
            set
            {
                _bestSector2 = value;
                OnPropertyChanged();
            }
        }

        public string BestSector3
        {
            get => _bestSector3;
            set
            {
                _bestSector3 = value;
                OnPropertyChanged();
            }
        }

        public string BestLap
        {
            get => _bestLap;
            set
            {
                _bestLap = value;
                OnPropertyChanged();
            }
        }

        public string SessionRemaining
        {
            get => _sessionRemaining;
            set
            {
                _sessionRemaining = value;
                OnPropertyChanged();
            }
        }

        public bool AnySectorFilled
        {
            get => _anySectorFilled;
            set
            {
                _anySectorFilled = value;
                OnPropertyChanged();
            }
        }

        private void RefreshAll()
        {
            if (SessionTiming == null)
            {
                return;
            }
            BestSector1 = FormatSectorTime(SessionTiming.BestSector1);
            BestSector2 = FormatSectorTime(SessionTiming.BestSector2);
            BestSector3 = FormatSectorTime(SessionTiming.BestSector3);
            BestLap = FormatBestLap(SessionTiming.BestSessionLap);
        }

        private void UpdateAnySectorsFilled()
        {
            AnySectorFilled = !(string.IsNullOrEmpty(BestSector1) && string.IsNullOrEmpty(BestSector2)
                                     && string.IsNullOrEmpty(BestSector3));
        }

        public SessionTiming SessionTiming
        {
            get => _timing;

            set
            {
                if (SessionTiming != null)
                {
                    SessionTiming.PropertyChanged -= SessionTimingOnPropertyChanged;
                }
                _timing = value;
                if (SessionTiming != null)
                {
                    SessionTiming.PropertyChanged += SessionTimingOnPropertyChanged;
                }
                RefreshAll();
                UpdateAnySectorsFilled();
            }
        }

        private void SessionTimingOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(SessionTiming.BestSector1):
                    BestSector1 = FormatSectorTime(SessionTiming.BestSector1);
                    break;
                case nameof(SessionTiming.BestSector2):
                    BestSector2 = FormatSectorTime(SessionTiming.BestSector2);
                    break;
                case nameof(SessionTiming.BestSector3):
                    BestSector3 = FormatSectorTime(SessionTiming.BestSector3);
                    break;
                case nameof(SessionTiming.BestSessionLap):
                    BestLap = FormatBestLap(SessionTiming.BestSessionLap);
                    break;
            }

            UpdateAnySectorsFilled();
        }

        private string FormatSectorTime(SectorTiming sectorTiming)
        {
            if (sectorTiming == null)
            {
                return string.Empty;
            }
            return "S" + sectorTiming.SectorNumber + ":" + sectorTiming.Lap.Driver.DriverInfo.DriverName + "-(L" + sectorTiming.Lap.LapNumber + "):"
                   + TimeSpanFormatHelper.FormatTimeSpanOnlySeconds(sectorTiming.Duration, false);
        }

        private string FormatBestLap(LapInfo bestLap)
        {
            if (bestLap == null)
            {
                return "No Best Lap";
            }

            return bestLap.Driver.DriverInfo.DriverName + "-(L" + bestLap.LapNumber + "):"
                   + bestLap.LapTime.FormatToDefault();
        }

        private string GetSessionRemaining(SimulatorDataSet dataSet)
        {
            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Na)
            {
                return "NA";
            }

            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Time)
            {
                string timeRemaining = "Time Remaining: " + GetTimeRemaining(dataSet).FormatToMinutesSeconds();
                if (_timing?.Leader != null && dataSet.SessionInfo?.SessionType == SessionType.Race && _timing?.Leader?.DriverTiming?.Pace != TimeSpan.Zero)
                {
                    timeRemaining += "\nEst. Laps:" + (Math.Floor(GetLapsRemaining(dataSet) * 10) / 10.0).ToString("N1");
                }

                return timeRemaining;
            }

            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Laps)
            {
                int lapsToGo = dataSet.SessionInfo.TotalNumberOfLaps - dataSet.SessionInfo.LeaderCurrentLap + 1;
                if (lapsToGo < 1)
                {
                    return "Leader Finished";
                }
                if (lapsToGo == 1)
                {
                    return "Leader on Final Lap";
                }
                string lapsToDisplay = lapsToGo < 2000
                                           ? lapsToGo.ToString()
                                           : "Infinite";
                if (_timing?.Leader != null && dataSet.SessionInfo?.SessionType == SessionType.Race && _timing?.Leader?.DriverTiming?.Pace != TimeSpan.Zero)
                {
                    lapsToDisplay += "\nEst. Time: " + GetTimeRemaining(dataSet).FormatToMinutesSeconds();
                }
                return "Leader laps to go: " + lapsToDisplay;
            }

            return "NA";
        }

        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
            SessionRemaining = GetSessionRemaining(dataSet);
        }

        public void Reset()
        {

        }

        private TimeSpan GetTimeRemaining(SimulatorDataSet dataSet)
        {
            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Time)
            {
                return TimeSpan.FromSeconds(dataSet.SessionInfo.SessionTimeRemaining);
            }
            else
            {
                return SessionTiming?.Leader?.DriverTiming?.Pace != null
                    ? TimeSpan.FromSeconds(GetLeaderLapsToGo(dataSet) * SessionTiming.Leader.DriverTiming.Pace.TotalSeconds)
                    : TimeSpan.Zero;
            }
        }

        private double GetLapsRemaining(SimulatorDataSet dataSet)
        {

            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Laps)
            {
                return GetLeaderLapsToGo(dataSet);
            }
            else
            {
                double secondsTillSessionEnds = dataSet.LeaderInfo.IsPlayer ? dataSet.SessionInfo.SessionTimeRemaining : GetSecondsTillLeaderFinished(dataSet);
                double distanceToGo = (secondsTillSessionEnds / _timing.Player.DriverTiming.Pace.TotalSeconds) * dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters;
                double distanceWithLapDistance = distanceToGo + dataSet.PlayerInfo.LapDistance;
                double distanceToFinishLap = dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters - ((int)distanceWithLapDistance % (int)dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters);
                double totalDistanceToGo = distanceToGo + distanceToFinishLap;
                return totalDistanceToGo / dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters;
            }
        }

        private double GetSecondsTillLeaderFinished(SimulatorDataSet dataSet)
        {
            double distanceToGo = (dataSet.SessionInfo.SessionTimeRemaining /
                                   _timing.Leader.DriverTiming.Pace.TotalSeconds) * dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters;
            double distanceWithLapDistance = distanceToGo + dataSet.LeaderInfo.LapDistance;
            double distanceToFinishLap = dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters - ((int)distanceWithLapDistance % (int)dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters);
            double totalDistanceToGo = distanceToGo + distanceToFinishLap;
            double totalLapsToGo = totalDistanceToGo / dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters;
            return totalLapsToGo * _timing.Leader.DriverTiming.Pace.TotalSeconds;
        }

        private double GetLeaderLapsToGo(SimulatorDataSet dataSet)
        {
            double fullLapsToGo = dataSet.SessionInfo.TotalNumberOfLaps - dataSet.SessionInfo.LeaderCurrentLap + 1;
            return fullLapsToGo - (dataSet.LeaderInfo.LapDistance / dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}