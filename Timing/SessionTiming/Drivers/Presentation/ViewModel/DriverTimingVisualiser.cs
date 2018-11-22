using System.Windows;
using System.Windows.Media;

namespace SecondMonitor.Timing.SessionTiming.Drivers.Presentation.ViewModel
{
    using System;
    using System.Windows.Input;
    using WindowsControls.WPF.Commands;
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

        public SolidColorBrush OutlineBrush
        {
            get;
            private set;
        }

        public bool ColorLapsColumns
        {
            get;
            private set;
        }

        public bool IsLastPlayerLapBetter
        {
            get;
            private set;
        }

        public bool IsPlayersPaceBetter
        {
            get;
            private set;
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

        public string Position
        {
            get;
            private set;
        }

        public string CarName
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public string CompletedLaps
        {
            get;
            private set;
        }

        public string LastLapTime
        {
            get;
            private set;
        }

        public string CurrentLapProgressTime
        {
            get;
            private set;
        }

        public string Pace
        {
            get;
            private set;
        }

        public string BestLap
        {
            get;
            private set;
        }

        public string LastPitInfo
        {
            get;
            private set;
        }

        public string Remark
        {
            get;
            private set;
        }

        public string TimeToPlayer
        {
            get;
            private set;
        }

        public string TopSpeed
        {
            get;
            private set;
        }

        public bool IsPlayer
        {
            get;
            private set;
        }

        public bool IsLapped
        {
            get;
            private set;
        }

        public bool IsLapping
        {
            get;
            private set;
        }

        public bool InPits
        {
            get;
            private set;
        }

        public bool IsLastLapBestLap
        {
            get;
            private set;
        }

        public bool IsLastLapBestSessionLap
        {
            get;
            private set;
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

        public string Sector1
        {
            get;
            private set;
        }

        public string Sector2
        {
            get;
            private set;
        }

        public string Sector3
        {
            get;
            private set;
        }

        public bool IsLastSector1PersonalBest
        {
            get;
            private set;
        }

        public bool IsLastSector2PersonalBest
        {
            get;
            private set;
        }

        public bool IsLastSector3PersonalBest
        {
            get;
            private set;
        }

        public bool IsLastSector1SessionBest
        {
            get;
            private set;
        }

        public bool IsLastSector2SessionBest
        {
            get;
            private set;
        }

        public bool IsLastSector3SessionBest
        {
            get;
            private set;
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
                NotifyPropertyChanged(string.Empty);
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
