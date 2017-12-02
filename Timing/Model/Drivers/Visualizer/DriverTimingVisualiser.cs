using SecondMonitor.DataModel.Drivers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using NLog;
using SecondMonitor.DataModel;
using SecondMonitor.Timing.Annotations;
using SecondMonitor.Timing.Model.Settings.ModelView;

namespace SecondMonitor.Timing.Model.Drivers.Visualizer
{
    public class DriverTimingVisualizer : DependencyObject
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position",typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty CarNameProperty = DependencyProperty.Register("CarName", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty CompletedLapsProperty = DependencyProperty.Register("CompletedLaps", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty LastLapTimeProperty = DependencyProperty.Register("LastLapTime", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty CurrentLapProgressTimeProperty = DependencyProperty.Register("CurrentLapProgressTime", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty PaceProperty = DependencyProperty.Register("Pace", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty BestLapProperty = DependencyProperty.Register("BestLap", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty LastPitInfoProperty = DependencyProperty.Register("LastPitInfo", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty RemarkProperty = DependencyProperty.Register("Remark", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty TimeToPlayerProperty = DependencyProperty.Register("TimeToPlayer", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty TopSpeedProperty = DependencyProperty.Register("TopSpeed", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty IsPlayerProperty = DependencyProperty.Register("IsPlayer", typeof(bool), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty IsLappedProperty = DependencyProperty.Register("IsLapped", typeof(bool), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty IsLappingProperty = DependencyProperty.Register("IsLapping", typeof(bool), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty InPitsProperty = DependencyProperty.Register("InPits", typeof(bool), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty IsLastLapBestLapProperty = DependencyProperty.Register("IsLastLapBestLap", typeof(bool), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty IsLastLapBestSessionLapProperty = DependencyProperty.Register("IsLastLapBestSessionLap", typeof(bool), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty DisplaySettingModelViewProperty = DependencyProperty.Register("DisplaySettingModelView", typeof(DisplaySettingsModelView), typeof(DriverTimingVisualizer));
      

        public DriverTimingVisualizer(DriverTiming driverTiming)
        {
            DriverTiming = driverTiming;
            ScheduleRefresh(this, CancellationToken.None);
        } 
        private DriverTiming _driverTiming;
        private bool _shouldRefresh = false;

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

        private static async Task ScheduleRefresh(DriverTimingVisualizer sender, CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested)
            {
                int refreshRate = 1000;
                if(sender.DisplaySettingsModelView!=null)
                    refreshRate = sender.DisplaySettingsModelView.RefreshRate;
                await Task.Delay(refreshRate, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                    sender._shouldRefresh = true;
            }
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

        public void RefreshProperties()
        {
            try
            {
                if (!_shouldRefresh)
                    return;
                Position = DriverTiming.Position.ToString();
                CompletedLaps = DriverTiming.CompletedLaps.ToString();
                LastLapTime = GetLastLapTime();
                CurrentLapProgressTime = GetCurrentLapProgressTime();
                Pace = GetPace();
                BestLap = GetBestLap();
                Remark = DriverTiming.Remark;
                LastPitInfo = DriverTiming.LastPitInfo;
                TopSpeed = GetTopSpeed();
                TimeToPlayer = GetTimeToPlayer();
                IsPlayer = DriverTiming.IsPlayer;
                IsLapped = DriverTiming.IsLapped;
                IsLapping = DriverTiming.IsLapping;
                InPits = DriverTiming.InPits;
                IsLastLapBestSessionLap = DriverTiming.IsLastLapBestSessionLap;
                IsLastLapBestLap = DriverTiming.IsLastLapBestLap;

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

        private string GetLastLapTime()
        {
            DriverInfo driverInfo = DriverTiming.DriverInfo;
            LapInfo lastCompletedLap = DriverTiming.LastCompletedLap;
            if (lastCompletedLap != null)
            {
                if (driverInfo.IsPlayer || !DriverTiming.Session.DisplayBindTimeRelative || DriverTiming.Session.Player.DriverTiming.LastCompletedLap == null)
                {
                    return FormatTimeSpan(lastCompletedLap.LapTime);
                }
                return FormatTimeSpanOnlySeconds(lastCompletedLap.LapTime.Subtract(DriverTiming.Session.Player.DriverTiming.LastCompletedLap.LapTime));
            }

            return "N/A";

        }

        private string GetCurrentLapProgressTime()
        {

            if (DriverTiming.CurrentLap == null)
            {
                return "";
            }

            if (!DriverTiming.CurrentLap.Valid)
            {
                return "Lap Invalid";
            }

            TimeSpan progress = DriverTiming.CurrentLap.LapProgressTime;
            return FormatTimeSpan(progress);

        }

        private string GetPace()
        {
            
                if (DriverTiming.DriverInfo.IsPlayer || !DriverTiming.Session.DisplayBindTimeRelative || DriverTiming.Session.Player.DriverTiming.Pace == TimeSpan.Zero)
                {
                    return FormatTimeSpan(DriverTiming.Pace);
                }
                else
                {
                    return FormatTimeSpanOnlySeconds(DriverTiming.Pace.Subtract(DriverTiming.Session.Player.DriverTiming.Pace));
                }
            
        }

        private string GetBestLap()
        {
            
                if (DriverTiming.BestLap == null)
                {
                    return "N/A";
                }

                if (DriverTiming.DriverInfo.IsPlayer || !DriverTiming.Session.DisplayBindTimeRelative || DriverTiming.Session.Player.DriverTiming.BestLap == null)
                {
                    return "L" + DriverTiming.BestLap.LapNumber + "/" + FormatTimeSpan(DriverTiming.BestLap.LapTime);
                }
                else
                {
                    return "L" + DriverTiming.BestLap.LapNumber + "/" + FormatTimeSpanOnlySeconds(DriverTiming.BestLap.LapTime.Subtract(DriverTiming.Session.Player.DriverTiming.BestLap.LapTime));
                }
            
        }

        public string GetTopSpeed()
        {
            return DriverTiming.TopSpeed.InKph.ToString("N0");
        }

        public string GetTimeToPlayer()
        {

            if (DriverTiming.Session.Player == null)
            {
                return "";
            }

            if (DriverTiming.DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.None && DriverTiming.DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.Na)
            {
                return DriverTiming.DriverInfo.FinishStatus.ToString();
            }

            if (DriverTiming.DriverInfo.IsPlayer)
            {
                return "";
            }

            if (DriverTiming.Session.LastSet.SessionInfo.SessionType != SessionInfo.SessionTypeEnum.Race)
            {
                return "";
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

            if (Math.Abs(distanceToUse) > DriverTiming.Session.LastSet.SessionInfo.LayoutLength)
            {
                return ((int)(distanceToUse) / (int)DriverTiming.Session.LastSet.SessionInfo.LayoutLength) + "LAP";
            }

            if (distanceToUse > 0)
            {
                double requiredTime = distanceToUse / (DriverTiming.DriverInfo.Speed.InMs);
                if (requiredTime < 30)
                {
                    return FormatTimeSpanOnlySeconds(TimeSpan.FromSeconds(requiredTime));
                }
                else
                {
                    return "+30.000+";
                }
            }
            else
            {
                double requiredTime = distanceToUse / (DriverTiming.Session.Player.DriverTiming.DriverInfo.Speed.InMs);
                if (requiredTime > -30)
                {
                    return FormatTimeSpanOnlySeconds(TimeSpan.FromSeconds(requiredTime));
                }
                else
                {
                    return "-30.000+";
                }
            }

        }


        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            //String seconds = timeSpan.Seconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            //String miliseconds = timeSpan.Milliseconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            //return timeSpan.Minutes + ":" + timeSpan.Seconds + "." + timeSpan.Milliseconds;
            return timeSpan.ToString("mm\\:ss\\.fff");
        }
        public static string FormatTimeSpanOnlySeconds(TimeSpan timeSpan)
        {
            //return "FOO";
            //String seconds = timeSpan.Seconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            //String miliseconds = timeSpan.Milliseconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            //return timeSpan.Minutes + ":" + timeSpan.Seconds + "." + timeSpan.Milliseconds;
            if (timeSpan < TimeSpan.Zero)
            {
                return "-" + timeSpan.ToString("ss\\.fff");
            }
            else
            {
                return "+" + timeSpan.ToString("ss\\.fff");
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
