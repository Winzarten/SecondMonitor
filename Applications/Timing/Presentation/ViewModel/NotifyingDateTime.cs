namespace SecondMonitor.Timing.Presentation.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Windows.Threading;

    public class NotifyingDateTime : INotifyPropertyChanged
    {
        private DateTime _now;

        public event PropertyChangedEventHandler PropertyChanged;
        
        public NotifyingDateTime()
        {
            _now = DateTime.Now;
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            timer.Tick += TimerTick;
            timer.Start();
        }

        public DateTime Now
        {
            get => _now;
            private set
            {
                _now = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Now"));
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Now = DateTime.Now;
        }

    }
}