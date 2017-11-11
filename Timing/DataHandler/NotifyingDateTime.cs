using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace SecondMonitor.Timing.DataHandler
{
    public class NotifyingDateTime : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private DateTime _now;
        public NotifyingDateTime()
        {
            _now = DateTime.Now;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        public DateTime Now
        {
            get { return _now; }
            private set
            {
                _now = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Now"));
            }

        }
        void timer_Tick(object sender, EventArgs e)
        {
            Now = DateTime.Now;
        }

    }
}