using SecondMonitor.DataModel.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SecondMonitor.Timing.Model.Drivers.Visualizer
{
    public class DriverTimingVisualizer : DependencyObject
    {
        /*
         *                    
                    <GridViewColumn DisplayMemberBinding="{Binding LastLapTime, Mode=OneWay}" Header="Last Lap" Width="150"/>
                    <GridViewColumn DisplayMemberBinding="{Binding PaceAsString, Mode=OneWay}" Header="Pace" Width="150"/>
                    <GridViewColumn DisplayMemberBinding="{Binding BestLapString, Mode=OneWay}" Header="Best Lap" Width="180"/>
                    <GridViewColumn DisplayMemberBinding="{Binding CurrentLapProgressTime, Mode=OneWay}" Header="Current Lap" Width="150"/>
                    <GridViewColumn DisplayMemberBinding="{Binding LastPitInfo, Mode=OneWay}" Header="Pits" Width="180"/>
                    <!--<GridViewColumn DisplayMemberBinding="{Binding Remark, Mode=OneWay}" Header="Remark" Width="180"/>-->
                    <GridViewColumn DisplayMemberBinding="{Binding TimeToPlayerFormatted, Mode=OneWay}" Header="Gap" Width="150"/>
                    <GridViewColumn DisplayMemberBinding="{Binding TopSpeedString, Mode=OneWay}" Header="Top Speed" Width="100"/>*/
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position",typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty CarNameProperty = DependencyProperty.Register("CarName", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty CompletedLapsProperty = DependencyProperty.Register("CompletedLaps", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty LastLapTimeProperty = DependencyProperty.Register("LastLapTime", typeof(string), typeof(DriverTimingVisualizer));
        public static readonly DependencyProperty CurrentLapProgressTimeProperty = DependencyProperty.Register("CurrentLapProgressTime", typeof(string), typeof(DriverTimingVisualizer));



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
                IntializeOneTimeValues();
            }
        }

        private static async Task ScheduleRefresh(DriverTimingVisualizer sender, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                    sender._shouldRefresh = true;
            }
        }

        private void IntializeOneTimeValues()
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

        public void RefreshProperties()
        {
            if (!_shouldRefresh)
                return;
            Position = DriverTiming.Position.ToString();            
            CompletedLaps = DriverTiming.CompletedLaps.ToString();
            LastLapTime = GetLastLapTime();
            CurrentLapProgressTime = GetCurrentLapProgressTime();
            _shouldRefresh = false;

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

        public string GetCurrentLapProgressTime()
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
        
    }

}
