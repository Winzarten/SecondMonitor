namespace SecondMonitor.Timing.LapTimings.ViewModel
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    using SecondMonitor.Timing.Presentation.ViewModel;
    using SecondMonitor.Timing.SessionTiming.Drivers.ModelView;

    public class LapViewModel : DependencyObject
    {
        public static readonly DependencyProperty LapNumberProperty = DependencyProperty.Register("LapNumber", typeof(int), typeof(LapViewModel));
        public static readonly DependencyProperty Sector1Property = DependencyProperty.Register("Sector1", typeof(string), typeof(LapViewModel));
        public static readonly DependencyProperty Sector2Property = DependencyProperty.Register("Sector2", typeof(string), typeof(LapViewModel));
        public static readonly DependencyProperty Sector3Property = DependencyProperty.Register("Sector3", typeof(string), typeof(LapViewModel));
        public static readonly DependencyProperty LapTimeProperty = DependencyProperty.Register("LapTime", typeof(string), typeof(LapViewModel));
        public static readonly DependencyProperty IsSector1PersonalBestProperty = DependencyProperty.Register("IsSector1PersonalBest", typeof(bool), typeof(LapViewModel));
        public static readonly DependencyProperty IsSector2PersonalBestProperty = DependencyProperty.Register("IsSector2PersonalBest", typeof(bool), typeof(LapViewModel));
        public static readonly DependencyProperty IsSector3PersonalBestProperty = DependencyProperty.Register("IsSector3PersonalBest", typeof(bool), typeof(LapViewModel));
        public static readonly DependencyProperty IsSector1SessionBestProperty = DependencyProperty.Register("IsSector1SessionBest", typeof(bool), typeof(LapViewModel));
        public static readonly DependencyProperty IsSector2SessionBestProperty = DependencyProperty.Register("IsSector2SessionBest", typeof(bool), typeof(LapViewModel));
        public static readonly DependencyProperty IsSector3SessionBestProperty = DependencyProperty.Register("IsSector3SessionBest", typeof(bool), typeof(LapViewModel));
        public static readonly DependencyProperty IsLapBestSessionLapProperty = DependencyProperty.Register("IsLapBestSessionLap", typeof(bool), typeof(LapViewModel));
        public static readonly DependencyProperty IsLapBestPersonalLapProperty = DependencyProperty.Register("IsLapBestPersonalLap", typeof(bool), typeof(LapViewModel));


        private bool _refresh = true;

        public LapViewModel(LapInfo lapInfo)
        {
            LapInfo = lapInfo;
            RefreshInfo();
            TimerMethod(RefreshInfo, () => LapInfo.Driver.Session.TimingDataViewModel.DisplaySettings.RefreshRate);
            
        }

        public void StopRefresh()
        {
            _refresh = false;
        }

        public LapInfo LapInfo
        {
            get;
            set;
        }

        public int LapNumber
        {
            get => (int)GetValue(LapNumberProperty);
            set => SetValue(LapNumberProperty, value);
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

        public string LapTime
        {
            get => (string)GetValue(LapTimeProperty);
            set => SetValue(LapTimeProperty, value);
        }

        public bool IsSector1PersonalBest
        {
            get => (bool)GetValue(IsSector1PersonalBestProperty);
            set => SetCurrentValue(IsSector1PersonalBestProperty, value);
        }

        public bool IsSector2PersonalBest
        {
            get => (bool)GetValue(IsSector2PersonalBestProperty);
            set => SetCurrentValue(IsSector2PersonalBestProperty, value);
        }

        public bool IsSector3PersonalBest
        {
            get => (bool)GetValue(IsSector3PersonalBestProperty);
            set => SetCurrentValue(IsSector3PersonalBestProperty, value);
        }

        public bool IsSector1SessionBest
        {
            get => (bool)GetValue(IsSector1SessionBestProperty);
            set => SetCurrentValue(IsSector1SessionBestProperty, value);
        }

        public bool IsSector2SessionBest
        {
            get => (bool)GetValue(IsSector2SessionBestProperty);
            set => SetCurrentValue(IsSector2SessionBestProperty, value);
        }

        public bool IsSector3SessionBest
        {
            get => (bool)GetValue(IsSector3SessionBestProperty);
            set => SetCurrentValue(IsSector3SessionBestProperty, value);
        }

        public bool IsLapBestSessionLap
        {
            get => (bool)GetValue(IsLapBestSessionLapProperty);
            set => SetValue(IsLapBestSessionLapProperty, value);
        }

        public bool IsLapBestPersonalLap
        {
            get => (bool)GetValue(IsLapBestPersonalLapProperty);
            set => SetValue(IsLapBestPersonalLapProperty, value);
        }


        private async void TimerMethod(Action timedAction, Func<int> delayAction)
        {
            while (_refresh)
            {
                await Task.Delay(delayAction() * 2);
                timedAction();
            }
        }

        private void RefreshInfo()
        {
            LapNumber = LapInfo.LapNumber;
            Sector1 = GetSector1();
            Sector2 = GetSector2();
            Sector3 = GetSector3();
            LapTime = GetLapTime();
            IsSector1SessionBest = GetIsSector1SessionBest();
            IsSector2SessionBest = GetIsSector2SessionBest();
            IsSector3SessionBest = GetIsSector3SessionBest();
            IsSector1PersonalBest = GetIsSector1PersonalBest();
            IsSector2PersonalBest = GetIsSector2PersonalBest();
            IsSector3PersonalBest = GetIsSector3PersonalBest();
            IsLapBestSessionLap = GetIsLapBestSessionLap();
            IsLapBestPersonalLap = GetIsLapBestPersonalLap();
        }

        private bool GetIsLapBestSessionLap()
        {
            return LapInfo == LapInfo.Driver.Session.BestSessionLap;
        }

        private bool GetIsLapBestPersonalLap()
        {
            return LapInfo == LapInfo.Driver.BestLap;
        }

        private bool GetIsSector1SessionBest()
        {
            var sector = LapInfo.Sector1;
            return sector != null && sector == LapInfo.Driver.Session.BestSector1;
        }

        private bool GetIsSector2SessionBest()
        {
            var sector = LapInfo.Sector2;
            return sector != null && sector == LapInfo.Driver.Session.BestSector2;
        }

        private bool GetIsSector3SessionBest()
        {
            var sector = LapInfo.Sector3;
            return sector != null && sector == LapInfo.Driver.Session.BestSector3;
        }

        private bool GetIsSector1PersonalBest()
        {
            var sector = LapInfo.Sector1;
            return sector != null && sector == LapInfo.Driver.BestSector1;
        }

        private bool GetIsSector2PersonalBest()
        {
            var sector = LapInfo.Sector2;
            return sector != null && sector == LapInfo.Driver.BestSector2;
        }

        private bool GetIsSector3PersonalBest()
        {
            var sector = LapInfo.Sector3;
            return sector != null && sector == LapInfo.Driver.BestSector3;
        }

        private string GetSectorTiming(SectorTiming sectorTiming)
        {
            if (!LapInfo.Valid)
            {
                return "Lap Invalid";
            }
            return sectorTiming == null ? "N/A" : TimeSpanFormatHelper.FormatTimeSpanOnlySeconds(sectorTiming.Duration, false);
        }

        private string GetSector1()
        {
            return GetSectorTiming(LapInfo.Sector1);
        }

        private string GetSector2()
        {
            return GetSectorTiming(LapInfo.Sector2);
        }

        private string GetSector3()
        {
            return GetSectorTiming(LapInfo.Sector3);
        }

        private string GetLapTime()
        {
            if (!LapInfo.Valid)
            {
                return "Lap Invalid";
            }
            return LapInfo.Completed ? TimeSpanFormatHelper.FormatTimeSpan(LapInfo.LapTime) : TimeSpanFormatHelper.FormatTimeSpan(LapInfo.LapProgressTime);
        }
    }
}