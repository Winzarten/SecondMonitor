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
            this.LapInfo = lapInfo;
            this.RefreshInfo();
            this.TimerMethod(this.RefreshInfo, () => this.LapInfo.Driver.Session.TimingDataViewModel.DisplaySettings.RefreshRate);
            
        }

        public void StopRefresh()
        {
            this._refresh = false;
        }

        public LapInfo LapInfo
        {
            get;
            set;
        }

        public int LapNumber
        {
            get => (int)this.GetValue(LapNumberProperty);
            set => this.SetValue(LapNumberProperty, value);
        }

        public string Sector1
        {
            get => (string)this.GetValue(Sector1Property);
            set => this.SetValue(Sector1Property, value);
        }

        public string Sector2
        {
            get => (string)this.GetValue(Sector2Property);
            set => this.SetValue(Sector2Property, value);
        }

        public string Sector3
        {
            get => (string)this.GetValue(Sector3Property);
            set => this.SetValue(Sector3Property, value);
        }

        public string LapTime
        {
            get => (string)this.GetValue(LapTimeProperty);
            set => this.SetValue(LapTimeProperty, value);
        }

        public bool IsSector1PersonalBest
        {
            get => (bool)this.GetValue(IsSector1PersonalBestProperty);
            set => this.SetCurrentValue(IsSector1PersonalBestProperty, value);
        }

        public bool IsSector2PersonalBest
        {
            get => (bool)this.GetValue(IsSector2PersonalBestProperty);
            set => this.SetCurrentValue(IsSector2PersonalBestProperty, value);
        }

        public bool IsSector3PersonalBest
        {
            get => (bool)this.GetValue(IsSector3PersonalBestProperty);
            set => this.SetCurrentValue(IsSector3PersonalBestProperty, value);
        }

        public bool IsSector1SessionBest
        {
            get => (bool)this.GetValue(IsSector1SessionBestProperty);
            set => this.SetCurrentValue(IsSector1SessionBestProperty, value);
        }

        public bool IsSector2SessionBest
        {
            get => (bool)this.GetValue(IsSector2SessionBestProperty);
            set => this.SetCurrentValue(IsSector2SessionBestProperty, value);
        }

        public bool IsSector3SessionBest
        {
            get => (bool)this.GetValue(IsSector3SessionBestProperty);
            set => this.SetCurrentValue(IsSector3SessionBestProperty, value);
        }

        public bool IsLapBestSessionLap
        {
            get => (bool)this.GetValue(IsLapBestSessionLapProperty);
            set => this.SetValue(IsLapBestSessionLapProperty, value);
        }

        public bool IsLapBestPersonalLap
        {
            get => (bool)this.GetValue(IsLapBestPersonalLapProperty);
            set => this.SetValue(IsLapBestPersonalLapProperty, value);
        }


        private async void TimerMethod(Action timedAction, Func<int> delayAction)
        {
            while (this._refresh)
            {
                await Task.Delay(delayAction() * 2);
                timedAction();
            }
        }

        private void RefreshInfo()
        {
            this.LapNumber = this.LapInfo.LapNumber;
            this.Sector1 = this.GetSector1();
            this.Sector2 = this.GetSector2();
            this.Sector3 = this.GetSector3();
            this.LapTime = this.GetLapTime();
            this.IsSector1SessionBest = this.GetIsSector1SessionBest();
            this.IsSector2SessionBest = this.GetIsSector2SessionBest();
            this.IsSector3SessionBest = this.GetIsSector3SessionBest();
            this.IsSector1PersonalBest = this.GetIsSector1PersonalBest();
            this.IsSector2PersonalBest = this.GetIsSector2PersonalBest();
            this.IsSector3PersonalBest = this.GetIsSector3PersonalBest();
            this.IsLapBestSessionLap = this.GetIsLapBestSessionLap();
            this.IsLapBestPersonalLap = this.GetIsLapBestPersonalLap();
        }

        private bool GetIsLapBestSessionLap()
        {
            return this.LapInfo == this.LapInfo.Driver.Session.BestSessionLap;
        }

        private bool GetIsLapBestPersonalLap()
        {
            return this.LapInfo == this.LapInfo.Driver.BestLap;
        }

        private bool GetIsSector1SessionBest()
        {
            var sector = this.LapInfo.Sector1;
            return sector != null && sector == this.LapInfo.Driver.Session.BestSector1;
        }

        private bool GetIsSector2SessionBest()
        {
            var sector = this.LapInfo.Sector2;
            return sector != null && sector == this.LapInfo.Driver.Session.BestSector2;
        }

        private bool GetIsSector3SessionBest()
        {
            var sector = this.LapInfo.Sector3;
            return sector != null && sector == this.LapInfo.Driver.Session.BestSector3;
        }

        private bool GetIsSector1PersonalBest()
        {
            var sector = this.LapInfo.Sector1;
            return sector != null && sector == this.LapInfo.Driver.BestSector1;
        }

        private bool GetIsSector2PersonalBest()
        {
            var sector = this.LapInfo.Sector2;
            return sector != null && sector == this.LapInfo.Driver.BestSector2;
        }

        private bool GetIsSector3PersonalBest()
        {
            var sector = this.LapInfo.Sector3;
            return sector != null && sector == this.LapInfo.Driver.BestSector3;
        }

        private string GetSectorTiming(SectorTiming sectorTiming)
        {
            if (!this.LapInfo.Valid)
            {
                return "Lap Invalid";
            }
            return sectorTiming == null ? "N/A" : TimeSpanFormatHelper.FormatTimeSpanOnlySeconds(sectorTiming.Duration, false);
        }

        private string GetSector1()
        {
            return this.GetSectorTiming(this.LapInfo.Sector1);
        }

        private string GetSector2()
        {
            return this.GetSectorTiming(this.LapInfo.Sector2);
        }

        private string GetSector3()
        {
            return this.GetSectorTiming(this.LapInfo.Sector3);
        }

        private string GetLapTime()
        {
            if (!this.LapInfo.Valid)
            {
                return "Lap Invalid";
            }
            return this.LapInfo.Completed ? TimeSpanFormatHelper.FormatTimeSpan(this.LapInfo.LapTime) : TimeSpanFormatHelper.FormatTimeSpan(this.LapInfo.LapProgressTime);
        }
    }
}