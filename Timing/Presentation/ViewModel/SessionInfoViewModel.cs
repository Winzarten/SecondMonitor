namespace SecondMonitor.Timing.Presentation.ViewModel
{
    using System.ComponentModel;
    using System.Windows;

    using SecondMonitor.Timing.SessionTiming.Drivers.ModelView;
    using SecondMonitor.Timing.SessionTiming.ViewModel;

    public class SessionInfoViewModel : DependencyObject
    {

        public static readonly DependencyProperty BestSector1Property = DependencyProperty.Register("BestSector1", typeof(string), typeof(SessionInfoViewModel));
        public static readonly DependencyProperty BestSector2Property = DependencyProperty.Register("BestSector2", typeof(string), typeof(SessionInfoViewModel));
        public static readonly DependencyProperty BestSector3Property = DependencyProperty.Register("BestSector3", typeof(string), typeof(SessionInfoViewModel));
        public static readonly DependencyProperty AnySectorFilledProperty = DependencyProperty.Register("AnySectorFilled", typeof(bool), typeof(SessionInfoViewModel));
        public static readonly DependencyProperty BestLapProperty = DependencyProperty.Register("BestLap", typeof(string), typeof(SessionInfoViewModel));
        public static readonly DependencyProperty SessionRemainingProperty = DependencyProperty.Register("SessionRemaining", typeof(string), typeof(SessionInfoViewModel));

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
            get => (string)GetValue(BestSector1Property);
            set => SetValue(BestSector1Property, value);
        }

        public string BestSector2
        {
            get => (string)GetValue(BestSector2Property);
            set => SetValue(BestSector2Property, value);
        }

        public string BestSector3
        {
            get => (string)GetValue(BestSector3Property);
            set => SetValue(BestSector3Property, value);
        }

        public string BestLap
        {
            get => (string)GetValue(BestLapProperty);
            set => SetValue(BestLapProperty, value);
        }

        public string SessionRemaining
        {
            get => (string)GetValue(SessionRemainingProperty);
            set => SetValue(SessionRemainingProperty, value);
        }

        public bool AnySectorFilled
        {
            get => (bool)GetValue(AnySectorFilledProperty);
            set => SetValue(AnySectorFilledProperty, value);
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
            if (propertyChangedEventArgs.PropertyName == nameof(SessionTiming.BestSector1))
            {
                BestSector1 = FormatSectorTime(SessionTiming.BestSector1);
            }
            if (propertyChangedEventArgs.PropertyName == nameof(SessionTiming.BestSector2))
            {
                BestSector2 = FormatSectorTime(SessionTiming.BestSector2);
            }
            if (propertyChangedEventArgs.PropertyName == nameof(SessionTiming.BestSector3))
            {
                BestSector3 = FormatSectorTime(SessionTiming.BestSector3);
            }
            if (propertyChangedEventArgs.PropertyName == nameof(SessionTiming.BestSessionLap))
            {
                BestLap = FormatBestLap(SessionTiming.BestSessionLap);
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
                   + DriverTiming.FormatTimeSpan(bestLap.LapTime);
        }        
    }
}