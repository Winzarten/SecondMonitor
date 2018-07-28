namespace SecondMonitor.Timing.SessionTiming.Drivers.Presentation.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;

    using NLog;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot.Drivers;
    using SecondMonitor.Timing.Presentation.ViewModel;
    using SecondMonitor.Timing.Properties;
    using SecondMonitor.Timing.SessionTiming.Drivers.ModelView;
    using SecondMonitor.Timing.Settings.ModelView;

    public class DriverTimingModelView : DependencyObject
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty CarNameProperty = DependencyProperty.Register("CarName", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty CompletedLapsProperty = DependencyProperty.Register("CompletedLaps", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty LastLapTimeProperty = DependencyProperty.Register("LastLapTime", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty Sector1Property = DependencyProperty.Register("Sector1", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty Sector2Property = DependencyProperty.Register("Sector2", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty Sector3Property = DependencyProperty.Register("Sector3", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty CurrentLapProgressTimeProperty = DependencyProperty.Register("CurrentLapProgressTime", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty PaceProperty = DependencyProperty.Register("Pace", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty BestLapProperty = DependencyProperty.Register("BestLap", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty LastPitInfoProperty = DependencyProperty.Register("LastPitInfo", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty RemarkProperty = DependencyProperty.Register("Remark", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty TimeToPlayerProperty = DependencyProperty.Register("TimeToPlayer", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty TopSpeedProperty = DependencyProperty.Register("TopSpeed", typeof(string), typeof(DriverTimingModelView));
        public static readonly DependencyProperty IsPlayerProperty = DependencyProperty.Register("IsPlayer", typeof(bool), typeof(DriverTimingModelView));
        public static readonly DependencyProperty IsLappedProperty = DependencyProperty.Register("IsLapped", typeof(bool), typeof(DriverTimingModelView));
        public static readonly DependencyProperty IsLappingProperty = DependencyProperty.Register("IsLapping", typeof(bool), typeof(DriverTimingModelView));
        public static readonly DependencyProperty InPitsProperty = DependencyProperty.Register("InPits", typeof(bool), typeof(DriverTimingModelView));
        public static readonly DependencyProperty IsLastLapBestLapProperty = DependencyProperty.Register("IsLastLapBestLap", typeof(bool), typeof(DriverTimingModelView));
        public static readonly DependencyProperty IsLastSector1PersonalBestProperty = DependencyProperty.Register("IsLastSector1PersonalBest", typeof(bool), typeof(DriverTimingModelView));
        public static readonly DependencyProperty IsLastSector2PersonalBestProperty = DependencyProperty.Register("IsLastSector2PersonalBest", typeof(bool), typeof(DriverTimingModelView));
        public static readonly DependencyProperty IsLastSector3PersonalBestProperty = DependencyProperty.Register("IsLastSector3PersonalBest", typeof(bool), typeof(DriverTimingModelView));
        public static readonly DependencyProperty IsLastSector1SessionBestProperty = DependencyProperty.Register("IsLastSector1SessionBest", typeof(bool), typeof(DriverTimingModelView));
        public static readonly DependencyProperty IsLastSector2SessionBestProperty = DependencyProperty.Register("IsLastSector2SessionBest", typeof(bool), typeof(DriverTimingModelView));
        public static readonly DependencyProperty IsLastSector3SessionBestProperty = DependencyProperty.Register("IsLastSector3SessionBest", typeof(bool), typeof(DriverTimingModelView));
        public static readonly DependencyProperty IsLastLapBestSessionLapProperty = DependencyProperty.Register("IsLastLapBestSessionLap", typeof(bool), typeof(DriverTimingModelView));
        public static readonly DependencyProperty DisplaySettingModelViewProperty = DependencyProperty.Register("DisplaySettingModelView", typeof(DisplaySettingsModelView), typeof(DriverTimingModelView));


        private DriverTiming _driverTiming;

        private bool _shouldRefresh;

        public DriverTimingModelView(DriverTiming driverTiming)
        {
            DriverTiming = driverTiming;
            ScheduleRefresh(this, CancellationToken.None);
        }

        private static async void ScheduleRefresh(DriverTimingModelView sender, CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested)
            {
                int refreshRate = 1000;
                if (sender.DisplaySettingsModelView != null)
                {
                    refreshRate = sender.DisplaySettingsModelView.RefreshRate;
                }

                await Task.Delay(refreshRate, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    sender._shouldRefresh = true;
                }
            }
        }

        public DriverTiming DriverTiming
        {
            get => _driverTiming;
            set
            {
                _driverTiming = value;
                InitializeOneTimeValues();
                CreateBinding();
            }
        }

        private void CreateBinding()
        {
            Binding newBinding = new Binding("DisplaySettings");
            newBinding.Mode = BindingMode.OneWay;
            newBinding.Source = _driverTiming.Session.TimingDataViewModel;
            BindingOperations.SetBinding(this, DisplaySettingModelViewProperty, newBinding);
        }

        private void InitializeOneTimeValues()
        {
            CarName = DriverTiming.CarName;
            Name = DriverTiming.Name;
        }

        public string Position
        {
            get => (string) GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public string CarName
        {
            get => (string)GetValue(CarNameProperty);
            set => SetValue(CarNameProperty, value);
        }

        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public string CompletedLaps
        {
            get => (string)GetValue(CompletedLapsProperty);
            set => SetValue(CompletedLapsProperty, value);
        }

        public string LastLapTime
        {
            get => (string)GetValue(LastLapTimeProperty);
            set => SetValue(LastLapTimeProperty, value);
        }

        public string CurrentLapProgressTime
        {
            get => (string)GetValue(CurrentLapProgressTimeProperty);
            set => SetValue(CurrentLapProgressTimeProperty, value);
        }

        public string Pace
        {
            get => (string)GetValue(PaceProperty);
            set => SetValue(PaceProperty, value);
        }

        public string BestLap
        {
            get => (string)GetValue(BestLapProperty);
            set => SetValue(BestLapProperty, value);
        }

        public string LastPitInfo
        {
            get => (string)GetValue(LastPitInfoProperty);
            set => SetValue(LastPitInfoProperty, value);
        }

        public string Remark
        {
            get => (string)GetValue(RemarkProperty);
            set => SetValue(RemarkProperty, value);
        }

        public string TimeToPlayer
        {
            get => (string)GetValue(TimeToPlayerProperty);
            set => SetValue(TimeToPlayerProperty, value);
        }

        public string TopSpeed
        {
            get => (string)GetValue(TopSpeedProperty);
            set => SetValue(TopSpeedProperty, value);
        }

        public bool IsPlayer
        {
            get => (bool)GetValue(IsPlayerProperty);
            set => SetValue(IsPlayerProperty, value);
        }

        public bool IsLapped
        {
            get => (bool)GetValue(IsLappedProperty);
            set => SetValue(IsLappedProperty, value);
        }

        public bool IsLapping
        {
            get => (bool)GetValue(IsLappingProperty);
            set => SetValue(IsLappingProperty, value);
        }

        public bool InPits
        {
            get => (bool)GetValue(InPitsProperty);
            set => SetValue(InPitsProperty, value);
        }

        public bool IsLastLapBestLap
        {
            get => (bool)GetValue(IsLastLapBestLapProperty);
            set => SetValue(IsLastLapBestLapProperty, value);
        }

        public bool IsLastLapBestSessionLap
        {
            get => (bool)GetValue(IsLastLapBestSessionLapProperty);
            set => SetValue(IsLastLapBestSessionLapProperty, value);
        }

        public DisplaySettingsModelView DisplaySettingsModelView
        {
            get => (DisplaySettingsModelView) GetValue(DisplaySettingModelViewProperty);
            set => SetValue(DisplaySettingModelViewProperty, value);
        }

        public string Sector1
        {
            get => (string)GetValue(Sector1Property);
            set => SetValue(Sector1Property, value);
        }

        public string Sector2
        {
            get => (string)GetValue(Sector2Property);
            set => SetValue(Sector2Property, value);
        }

        public string Sector3
        {
            get => (string)GetValue(Sector3Property);
            set => SetValue(Sector3Property, value);
        }

        public bool IsLastSector1PersonalBest
        {
            get => (bool)GetValue(IsLastSector1PersonalBestProperty);
            set => SetCurrentValue(IsLastSector1PersonalBestProperty, value);
        }

        public bool IsLastSector2PersonalBest
        {
            get => (bool)GetValue(IsLastSector2PersonalBestProperty);
            set => SetCurrentValue(IsLastSector2PersonalBestProperty, value);
        }

        public bool IsLastSector3PersonalBest
        {
            get => (bool)GetValue(IsLastSector3PersonalBestProperty);
            set => SetCurrentValue(IsLastSector3PersonalBestProperty, value);
        }

        public bool IsLastSector1SessionBest
        {
            get => (bool)GetValue(IsLastSector1SessionBestProperty);
            set => SetCurrentValue(IsLastSector1SessionBestProperty, value);
        }

        public bool IsLastSector2SessionBest
        {
            get => (bool)GetValue(IsLastSector2SessionBestProperty);
            set => SetCurrentValue(IsLastSector2SessionBestProperty, value);
        }

        public bool IsLastSector3SessionBest
        {
            get => (bool)GetValue(IsLastSector3SessionBestProperty);
            set => SetCurrentValue(IsLastSector3SessionBestProperty, value);
        }

        public void RefreshProperties()
        {
            try
            {
                if (!_shouldRefresh)
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
                TopSpeed = GetTopSpeed().GetValueInUnits(DriverTiming.Session.TimingDataViewModel.DisplaySettings.VelocityUnits).ToString("N0");
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
                /*Sector1 = DriverTiming.DriverInfo.WorldPosition.X.DistanceInM.ToString("N2");
                Sector2 = DriverTiming.DriverInfo.WorldPosition.Y.DistanceInM.ToString("N2");
                Sector3 = DriverTiming.DriverInfo.WorldPosition.Z.DistanceInM.ToString("N2");
                BestLap = DriverTiming?.CurrentLap?.ComputedMaxSpeed?.InKph.ToString("N2");*/
                IsLastSector1SessionBest = GetIsSector1SessionBest();
                IsLastSector2SessionBest = GetIsSector2SessionBest();
                IsLastSector3SessionBest = GetIsSector3SessionBest();
                IsLastSector1PersonalBest = GetIsSector1PersonalBest();
                IsLastSector2PersonalBest = GetIsSector2PersonalBest();
                IsLastSector3PersonalBest = GetIsSector3PersonalBest();


            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
            }
            finally
            {
                _shouldRefresh = false;
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
            if (DriverTiming.CurrentLap == null || !DriverTiming.CurrentLap.Valid)
            {
                return "N/A";
            }
            var sector = GetSector1Timing();
            return sector == null ? "N/A" : TimeSpanFormatHelper.FormatTimeSpanOnlySeconds(sector.Duration, false);
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
            if (DriverTiming.CurrentLap == null || !DriverTiming.CurrentLap.Valid)
            {
                return "N/A";
            }
            SectorTiming sector = GetSector2Timing();
            return sector == null ? "N/A" : TimeSpanFormatHelper.FormatTimeSpanOnlySeconds(sector.Duration, false);
        }

        private string GetSector3()
        {
            if (DriverTiming.CurrentLap == null || !DriverTiming.CurrentLap.Valid)
            {
                return "N/A";
            }
            SectorTiming sector = GetSector3Timing();
            return sector == null ? "N/A" : TimeSpanFormatHelper.FormatTimeSpanOnlySeconds(sector.Duration, false);
        }

        private string GetLastLapTime()
        {
            DriverInfo driverInfo = DriverTiming.DriverInfo;
            LapInfo lastCompletedLap = DriverTiming.LastCompletedLap;
            if (lastCompletedLap != null)
            {
                if (driverInfo.IsPlayer || !DriverTiming.Session.DisplayBindTimeRelative || DriverTiming.Session.Player?.DriverTiming.LastCompletedLap == null)
                {
                    return TimeSpanFormatHelper.FormatTimeSpan(lastCompletedLap.LapTime);
                }

                return TimeSpanFormatHelper.FormatTimeSpanOnlySeconds(lastCompletedLap.LapTime.Subtract(DriverTiming.Session.Player.DriverTiming.LastCompletedLap.LapTime), true);
            }

            return "N/A";

        }

        private string GetCurrentLapProgressTime()
        {

            if (DriverTiming.CurrentLap == null)
            {
                return string.Empty;
            }

            if (!DriverTiming.CurrentLap.Valid)
            {
                return DriverTiming.Session.SessionType == SessionType.Race ? "Lap Invalid" : "Out Lap"; ;
            }

            TimeSpan progress = DriverTiming.CurrentLap.LapProgressTime;
            return TimeSpanFormatHelper.FormatTimeSpan(progress);

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
            if (DriverTiming?.Session?.Player?.DriverTiming?.BestLap == null)
            {
                return "N/A";
            }

                if (DriverTiming.BestLap == null)
                {
                    return "N/A";
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

        public Velocity GetTopSpeed()
        {
            return DriverTiming.TopSpeed;
        }

        public string GetTimeToPlayer()
        {

            if (DriverTiming.Session.Player == null)
            {
                return string.Empty;
            }

            if (DriverTiming.DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.None && DriverTiming.DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.Na)
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

            if (Math.Abs(distanceToUse) > DriverTiming.Session.LastSet.SessionInfo.TrackInfo.LayoutLength)
            {
                return ((int)distanceToUse / (int)DriverTiming.Session.LastSet.SessionInfo.TrackInfo.LayoutLength) + "LAP";
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
