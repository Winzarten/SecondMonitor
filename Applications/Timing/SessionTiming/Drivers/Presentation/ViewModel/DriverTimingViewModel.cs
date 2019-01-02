using System.Windows.Media;

namespace SecondMonitor.Timing.SessionTiming.Drivers.Presentation.ViewModel
{
    using System;
    using Annotations;
    using DataModel.BasicProperties;

    using NLog;

    using SecondMonitor.DataModel.Snapshot.Drivers;
    using SecondMonitor.Timing.Presentation.ViewModel;
    using SecondMonitor.Timing.SessionTiming.Drivers.ViewModel;
    using SimdataManagement.DriverPresentation;
    using ViewModels;
    using ViewModels.Settings.ViewModel;

    public class DriverTimingViewModel : AbstractViewModel
    {
       private DriverTiming _driverTiming;

        private DateTime _nextRefresh = DateTime.Now;
        private DisplaySettingsViewModel _displaySettingsViewModel;
        private readonly DriverPresentationsManager _driverPresentationsManager;
        private Color _outLineColor;
        private bool _hasCustomOutline;

        public DriverTimingViewModel(DriverTiming driverTiming, DisplaySettingsViewModel displaySettingsViewModel, DriverPresentationsManager driverPresentationsManager)
        {
            _displaySettingsViewModel = displaySettingsViewModel;
            _driverPresentationsManager = driverPresentationsManager;
            DriverTiming = driverTiming;
        }

        public bool HasCustomOutline
        {
            get => _hasCustomOutline && !IsPlayer;
            set
            {
                _hasCustomOutline = value;
                _driverPresentationsManager.SetOutLineColorEnabled(Name, value);
                NotifyPropertyChanged();
            }
        }

        public Color OutLineColor
        {
            get => _outLineColor;
            set
            {
                _outLineColor = value;
                OutlineBrush = new SolidColorBrush(_outLineColor);
                _driverPresentationsManager.SetOutLineColor(Name, value);
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(OutlineBrush));
            }
        }

        private SolidColorBrush _outLineBrush;
        public SolidColorBrush OutlineBrush
        {
            get => _outLineBrush;
            private set => SetProperty(ref _outLineBrush, value);
        }


        private bool _colorLapsColumns;
        public bool ColorLapsColumns
        {
            get => _colorLapsColumns;
            private set => SetProperty(ref _colorLapsColumns, value);
        }

        private bool _isLastPLayerLapBetter;
        public bool IsLastPlayerLapBetter
        {
            get => _isLastPLayerLapBetter;
            private set => SetProperty(ref _isLastPLayerLapBetter, value);
        }

        private bool _isPlayersPaceBetter;
        public bool IsPlayersPaceBetter
        {
            get => _isPlayersPaceBetter;
            private set => SetProperty(ref _isPlayersPaceBetter, value);
        }

        public DriverTiming DriverTiming
        {
            get => _driverTiming;
            set
            {
                _driverTiming = value;
                InitializeOneTimeValues();
            }
        }

        private string _position;
        public string Position
        {
            get => _position;
            private set => SetProperty(ref _position, value);
        }

        private string _positionInClass;
        public string PositionInClass
        {
            get => _positionInClass;
            private set => SetProperty(ref _positionInClass, value);
        }

        private string _carName;
        public string CarName
        {
            get => _carName;
            private set => SetProperty(ref _carName, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            private set => SetProperty(ref _name, value);
        }

        private string _completedLaps;
        public string CompletedLaps
        {
            get => _completedLaps;
            private set => SetProperty(ref _completedLaps, value);
        }

        private string _lastLapTime;
        public string LastLapTime
        {
            get => _lastLapTime;
            private set => SetProperty(ref _lastLapTime, value);
        }

        private string _currentLapProgressTime;
        public string CurrentLapProgressTime
        {
            get => _currentLapProgressTime;
            private set => SetProperty(ref _currentLapProgressTime, value);
        }


        private string _pace;
        public string Pace
        {
            get => _pace;
            private set => SetProperty(ref _pace, value);
        }

        private string _bestLap;
        public string BestLap
        {
            get => _bestLap;
            private set => SetProperty(ref _bestLap, value);
        }

        private string _lastPitInfo;
        public string LastPitInfo
        {
            get => _lastPitInfo;
            private set => SetProperty(ref _lastPitInfo, value);
        }

        private string _remark;
        public string Remark
        {
            get => _remark;
            private set => SetProperty(ref _remark, value);
        }

        private string _timeToPlayer;
        public string TimeToPlayer
        {
            get => _timeToPlayer;
            private set => SetProperty(ref _timeToPlayer, value);
        }

        private string _topSpeed;
        public string TopSpeed
        {
            get => _topSpeed;
            private set => SetProperty(ref _topSpeed, value);
        }

        private bool _isPlayer;
        public bool IsPlayer
        {
            get => _isPlayer;
            private set => SetProperty(ref _isPlayer, value);
        }

        private bool _isLapped;
        public bool IsLapped
        {
            get => _isLapped;
            private set => SetProperty(ref _isLapped, value);
        }

        private bool _isLapping;
        public bool IsLapping
        {
            get => _isLapping;
            private set => SetProperty(ref _isLapping, value);
        }

        private bool _inPits;
        public bool InPits
        {
            get => _inPits;
            private set => SetProperty(ref _inPits, value);
        }

        private bool _isLastLapBestLap;
        public bool IsLastLapBestLap
        {
            get => _isLastLapBestLap;
            private set => SetProperty(ref _isLastLapBestLap, value);
        }

        private bool _isLastLapBestSessionLap;
        public bool IsLastLapBestSessionLap
        {
            get => _isLastLapBestSessionLap;
            private set => SetProperty(ref _isLastLapBestSessionLap, value);
        }

        private string _carClassName;
        public string CarClassName
        {
            get => _carClassName;
            private set => SetProperty(ref _carClassName, value);
        }

        public DisplaySettingsViewModel DisplaySettingsViewModel
        {
            get => _displaySettingsViewModel;
            set
            {
                _displaySettingsViewModel = value;
                NotifyPropertyChanged();
            }
        }

        private string _sector1;
        public string Sector1
        {
            get => _sector1;
            private set => SetProperty(ref _sector1, value);
        }

        private string _sector2;
        public string Sector2
        {
            get => _sector2;
            private set => SetProperty(ref _sector2, value);
        }

        private string _sector3;
        public string Sector3
        {
            get => _sector3;
            private set => SetProperty(ref _sector3, value);
        }

        private bool _isLastSector1PersonalBest;
        public bool IsLastSector1PersonalBest
        {
            get => _isLastSector1PersonalBest;
            private set => SetProperty(ref _isLastSector1PersonalBest, value);
        }

        private bool _isLastSector2PersonalBest;
        public bool IsLastSector2PersonalBest
        {
            get => _isLastSector2PersonalBest;
            private set => SetProperty(ref _isLastSector2PersonalBest, value);
        }

        private bool _isLastSector3PersonalBest;
        public bool IsLastSector3PersonalBest
        {
            get => _isLastSector3PersonalBest;
            private set => SetProperty(ref _isLastSector3PersonalBest, value);
        }

        private bool _isLastSector1SessionBest;
        public bool IsLastSector1SessionBest
        {
            get => _isLastSector1SessionBest;
            private set => SetProperty(ref _isLastSector1SessionBest, value);
        }

        private bool _isLastSector2SessionBest;
        public bool IsLastSector2SessionBest
        {
            get => _isLastSector2SessionBest;
            private set => SetProperty(ref _isLastSector2SessionBest, value);
        }

        private bool _isLastSector3SessionBest;
        public bool IsLastSector3SessionBest
        {
            get => _isLastSector3SessionBest;
            private set => SetProperty(ref _isLastSector3SessionBest, value);
        }

        public void RefreshProperties()
        {
            try
            {
                if (DateTime.Now < _nextRefresh)
                {
                    return;
                }

                Position = DriverTiming.Position.ToString();
                PositionInClass = DriverTiming.PositionInClass > 0 && DriverTiming.PositionInClass != DriverTiming.Position ? $"{DriverTiming.PositionInClass.ToString()} ({Position})" : Position;
                CompletedLaps = DriverTiming.CompletedLaps.ToString();
                LastLapTime = GetLastLapTime();
                CurrentLapProgressTime = GetCurrentLapProgressTime();
                Pace = GetPace();
                BestLap = GetBestLap();
                Remark = DriverTiming.Remark;
                LastPitInfo = DriverTiming.LastPitInfo;
                TopSpeed = GetTopSpeed().GetValueInUnits(DriverTiming.Session.TimingDataViewModel.DisplaySettingsViewModel.VelocityUnits).ToString("N0");
                TimeToPlayer = GetTimeToPlayer();
                IsPlayer = DriverTiming.IsPlayer;
                IsLapped = DriverTiming.IsLapped;
                IsLapping = DriverTiming.IsLapping;
                InPits = DriverTiming.InPits;
                IsLastLapBestSessionLap = DriverTiming.IsLastLapBestSessionLap;
                IsLastLapBestLap = DriverTiming.IsLastLapBestLap;

                Sector1 = GetSector1();
                Sector2 = GetSector2();
                Sector3 = GetSector3();

                IsLastSector1SessionBest = GetIsSector1SessionBest();
                IsLastSector2SessionBest = GetIsSector2SessionBest();
                IsLastSector3SessionBest = GetIsSector3SessionBest();
                IsLastSector1PersonalBest = GetIsSector1PersonalBest();
                IsLastSector2PersonalBest = GetIsSector2PersonalBest();
                IsLastSector3PersonalBest = GetIsSector3PersonalBest();
                ColorLapsColumns = GetColorLapsColumns();
                IsLastPlayerLapBetter = GetIsLastPlayerLapBetter();
                IsPlayersPaceBetter = GetIsPlayersPaceBetter();
                _nextRefresh = DisplaySettingsViewModel != null ? DateTime.Now + TimeSpan.FromMilliseconds(DisplaySettingsViewModel.RefreshRate) : DateTime.Now + TimeSpan.FromSeconds(10);
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
            }
        }

        private bool GetColorLapsColumns()
        {
            return DriverTiming?.Session.SessionType == SessionType.Race && !DriverTiming.IsPlayer && DriverTiming?.Session?.Player?.DriverTiming?.CompletedLaps > 0;
        }

        private bool GetIsPlayersPaceBetter()
        {
            if (!ColorLapsColumns)
            {
                return false;
            }

            return DriverTiming.Pace > DriverTiming.Session.Player.DriverTiming.Pace;
        }

        private bool GetIsLastPlayerLapBetter()
        {
            if (!ColorLapsColumns || DriverTiming.LastCompletedLap is null || DriverTiming.Session.Player.DriverTiming.LastCompletedLap is null)
            {
                return false;
            }

            return DriverTiming.LastCompletedLap.LapTime > DriverTiming.Session.Player.DriverTiming.LastCompletedLap.LapTime;
        }

        private void InitializeOneTimeValues()
        {
            CarName = DriverTiming.CarName;
            CarClassName = DriverTiming.CarClassName;
            Name = DriverTiming.Name;
            HasCustomOutline = _driverPresentationsManager.IsCustomOutlineEnabled(Name);
            if (_driverPresentationsManager.TryGetOutLineColor(Name, out Color color))
            {
                OutLineColor = color;
            }
        }

        private bool GetIsSector1SessionBest()
        {
            if (DriverTiming.CurrentLap == null)
            {
                return false;
            }

            var sector = GetSector1Timing();
            return sector != null && sector == DriverTiming.Session.BestSector1;
        }

        private bool GetIsSector2SessionBest()
        {
            if (DriverTiming.CurrentLap == null)
            {
                return false;
            }

            var sector = GetSector2Timing();
            return sector != null && sector == DriverTiming.Session.BestSector2;
        }

        private bool GetIsSector3SessionBest()
        {
            if (DriverTiming.CurrentLap == null)
            {
                return false;
            }

            var sector = GetSector3Timing();
            return sector != null && sector == DriverTiming.Session.BestSector3;
        }

        private bool GetIsSector1PersonalBest()
        {
            if (DriverTiming.CurrentLap == null)
            {
                return false;
            }

            var sector = GetSector1Timing();
            return sector != null && sector == DriverTiming.BestSector1;
        }

        private bool GetIsSector2PersonalBest()
        {
            if (DriverTiming.CurrentLap == null)
            {
                return false;
            }

            var sector = GetSector2Timing();
            return sector != null && sector == DriverTiming.BestSector2;
        }

        private bool GetIsSector3PersonalBest()
        {
            if (DriverTiming.CurrentLap == null)
            {
                return false;
            }

            var sector = GetSector3Timing();
            return sector != null && sector == DriverTiming.BestSector3;
        }

        private string GetSector1()
        {
            if (DriverTiming.CurrentLap == null)
            {
                return "N/A";
            }
            var sector = GetSector1Timing();
            return FormatSectorTiming(sector);
        }

        private SectorTiming GetSector1Timing()
        {
            if (DriverTiming.CurrentLap == null)
            {
                return null;
            }
            SectorTiming sector = DriverTiming.CurrentLap.Sector1;
            if (sector == null && DriverTiming.CurrentLap.PreviousLap != null)
            {
                sector = DriverTiming.CurrentLap.PreviousLap.Sector1;
            }
            return sector;
        }

        private SectorTiming GetSector2Timing()
        {
            if (DriverTiming.CurrentLap == null)
            {
                return null;
            }
            SectorTiming sector = DriverTiming.CurrentLap.Sector2;
            if (sector == null && DriverTiming.CurrentLap.PreviousLap != null)
            {
                sector = DriverTiming.CurrentLap.PreviousLap.Sector2;
            }
            return sector;
        }

        private string FormatSectorTiming(SectorTiming sectorTiming)
        {
            if (sectorTiming == null)
            {
                return "N/A";
            }

            if (sectorTiming.Lap.Valid)
            {
                return TimeSpanFormatHelper.FormatTimeSpanOnlySeconds(sectorTiming.Duration, false);
            }

            if (!sectorTiming.Lap.Driver.Session.RetrieveAlsoInvalidLaps)
            {
                return "N/A";
            }

            if (sectorTiming.Lap.Driver.Session.SessionType == SessionType.Race || !sectorTiming.Lap.PitLap)
            {
                return TimeSpanFormatHelper.FormatTimeSpanOnlySeconds(sectorTiming.Duration, false);
            }

            return "N/A";
        }

        private SectorTiming GetSector3Timing()
        {
            if (DriverTiming.CurrentLap == null)
            {
                return null;
            }
            SectorTiming sector = DriverTiming.CurrentLap.Sector3;
            if (sector == null && DriverTiming.CurrentLap.PreviousLap != null)
            {
                sector = DriverTiming.CurrentLap.PreviousLap.Sector3;
            }
            return sector;
        }

        private string GetSector2()
        {
            if (DriverTiming.CurrentLap == null)
            {
                return "N/A";
            }
            SectorTiming sector = GetSector2Timing();
            return FormatSectorTiming(sector);
        }

        private string GetSector3()
        {
            if (DriverTiming.CurrentLap == null)
            {
                return "N/A";
            }
            SectorTiming sector = GetSector3Timing();
            return FormatSectorTiming(sector);
        }

        private string GetLastLapTime()
        {
            DriverInfo driverInfo = DriverTiming.DriverInfo;
            LapInfo lastCompletedLap = DriverTiming.LastCompletedLap;
            if (lastCompletedLap != null)
            {
                string toDisplay;
                if (!lastCompletedLap.Valid && DriverTiming.Session.SessionType != SessionType.Race)
                {
                    return "Lap Invalid";
                }

                if (lastCompletedLap.PitLap && DriverTiming.Session.SessionType != SessionType.Race)
                {
                    return "Out Lap";
                }

                if (driverInfo.IsPlayer || !DriverTiming.Session.DisplayBindTimeRelative || DriverTiming.Session.Player?.DriverTiming.LastCompletedLap == null)
                {
                    toDisplay = TimeSpanFormatHelper.FormatTimeSpan(lastCompletedLap.LapTime);
                }
                else
                {
                    toDisplay = TimeSpanFormatHelper.FormatTimeSpanOnlySeconds(lastCompletedLap.LapTime.Subtract(DriverTiming.Session.Player.DriverTiming.LastCompletedLap.LapTime), true);
                }

                return lastCompletedLap.Valid || DriverTiming.Session.SessionType != SessionType.Race ? toDisplay : "(I) " + toDisplay;

            }

            return "N/A";

        }

        private string GetCurrentLapProgressTime()
        {

            if (DriverTiming.CurrentLap == null)
            {
                return string.Empty;
            }

            TimeSpan progress = DriverTiming.CurrentLap.CurrentlyValidProgressTime;
            if (DriverTiming.Session.SessionType != SessionType.Race && (DriverTiming.CurrentLap.PitLap || DriverTiming.InPits))
            {
                return DriverTiming.InPits ? "In Pits" : "Out Lap";
            }

            if (!DriverTiming.CurrentLap.Valid && !DriverTiming.InPits && DriverTiming.Session.RetrieveAlsoInvalidLaps)
            {
                return "(I) " + TimeSpanFormatHelper.FormatTimeSpan(progress);
            }

            if (!DriverTiming.CurrentLap.Valid && !DriverTiming.Session.RetrieveAlsoInvalidLaps)
            {
                return DriverTiming.Session.SessionType == SessionType.Race ? "Lap Invalid" : DriverTiming.InPits ? "In Pits" : "Out Lap";
            }

            return DriverTiming.CurrentLap.Valid
                       ? TimeSpanFormatHelper.FormatTimeSpan(progress)
                       : "(I) " + TimeSpanFormatHelper.FormatTimeSpan(progress);

        }

        private string GetPace()
        {

            if (DriverTiming.Session.Player == null)
            {
                return string.Empty;
            }
            if (DriverTiming.DriverInfo.IsPlayer || !DriverTiming.Session.DisplayBindTimeRelative || DriverTiming.Session.Player.DriverTiming.Pace == TimeSpan.Zero)
            {
                return TimeSpanFormatHelper.FormatTimeSpan(DriverTiming.Pace);
            }
            else
            {
                return TimeSpanFormatHelper.FormatTimeSpanOnlySeconds(DriverTiming.Pace.Subtract(DriverTiming.Session.Player.DriverTiming.Pace), true);
            }

        }

        private string GetBestLap()
        {
            if (DriverTiming.BestLap == null)
            {
                return "N/A";
            }

            if (DriverTiming?.Session?.Player?.DriverTiming?.BestLap == null)
            {
                return "L" + DriverTiming.BestLap.LapNumber + "/" + TimeSpanFormatHelper.FormatTimeSpan(DriverTiming.BestLap.LapTime);
            }

            if (DriverTiming.DriverInfo.IsPlayer || !DriverTiming.Session.DisplayBindTimeRelative || DriverTiming.Session.Player.DriverTiming.BestLap == null)
            {
                return "L" + DriverTiming.BestLap.LapNumber + "/" + TimeSpanFormatHelper.FormatTimeSpan(DriverTiming.BestLap.LapTime);
            }
            else
            {
                return "L" + DriverTiming.BestLap.LapNumber + "/" + TimeSpanFormatHelper.FormatTimeSpanOnlySeconds(DriverTiming.BestLap.LapTime.Subtract(DriverTiming.Session.Player.DriverTiming.BestLap.LapTime), true);
            }

        }

        private Velocity GetTopSpeed()
        {
            return DriverTiming.TopSpeed;
        }

        private string GetTimeToPlayer()
        {

            if (DriverTiming.Session.Player == null)
            {
                return string.Empty;
            }

            if (DriverTiming.DriverInfo.FinishStatus != DriverFinishStatus.None && DriverTiming.DriverInfo.FinishStatus != DriverFinishStatus.Na)
            {
                return DriverTiming.DriverInfo.FinishStatus.ToString();
            }

            if (DriverTiming.DriverInfo.IsPlayer)
            {
                return string.Empty;
            }

            if (DriverTiming.Session.LastSet.SessionInfo.SessionType != SessionType.Race)
            {
                return string.Empty;
            }

            double distanceToUse;
            if (DriverTiming.Session.DisplayGapToPlayerRelative)
            {
                distanceToUse = DriverTiming.DriverInfo.DistanceToPlayer;
            }
            else
            {
                distanceToUse = DriverTiming.Session.Player.DriverTiming.TotalDistanceTraveled - DriverTiming.TotalDistanceTraveled;
            }

            if (Math.Abs(distanceToUse) > DriverTiming.Session.LastSet.SessionInfo.TrackInfo.LayoutLength.InMeters)
            {
                return ((int)distanceToUse / (int)DriverTiming.Session.LastSet.SessionInfo.TrackInfo.LayoutLength.InMeters) + "LAP";
            }

            if (distanceToUse > 0)
            {
                double requiredTime = distanceToUse / DriverTiming.DriverInfo.Speed.InMs;
                if (requiredTime < 30)
                {
                    return TimeSpanFormatHelper.FormatTimeSpanOnlySeconds(TimeSpan.FromSeconds(requiredTime), true);
                }
                else
                {
                    return "+30.000+";
                }
            }
            else
            {
                double requiredTime = distanceToUse / DriverTiming.Session.Player.DriverTiming.DriverInfo.Speed.InMs;
                if (requiredTime > -30)
                {
                    return TimeSpanFormatHelper.FormatTimeSpanOnlySeconds(TimeSpan.FromSeconds(requiredTime), true);
                }
                else
                {
                    return "-30.000+";
                }
            }

        }
    }

}
