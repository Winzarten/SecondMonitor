namespace SecondMonitor.Timing.LapTimings.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Input;

    using SecondMonitor.Timing.SessionTiming.Drivers.ModelView;

    using DriverLapsWindow = SecondMonitor.Timing.LapTimings.View.DriverLapsWindow;

    public class DriverLapsViewModel : DependencyObject
    {

        public static readonly DependencyProperty LapsProperty = DependencyProperty.Register("Laps", typeof(ObservableCollection<LapViewModel>), typeof(DriverLapsViewModel));
        public static readonly DependencyProperty DriverNameProperty = DependencyProperty.Register("DriverName", typeof(string), typeof(DriverLapsViewModel));
        

        private readonly DriverTiming _driverTiming;

        private readonly DriverLapsWindow _gui;

        public DriverLapsViewModel()
        {
            this.DriverName = "Design Time";
            this.Laps = new ObservableCollection<LapViewModel>();
        }

        public DriverTiming DriverTiming => this._driverTiming;

        public DriverLapsViewModel(DriverTiming driverTiming, DriverLapsWindow gui)
        {
            this._driverTiming = driverTiming;
            this.Laps = new ObservableCollection<LapViewModel>();
            this.BuildLapsViewModel();
            this._driverTiming.NewLapStarted += this.DriverTimingOnNewLapStarted;
            this.DriverName = this._driverTiming.Name;
            this._gui = gui;
            this._gui.Closed += this.GuiOnClosed;
            this._gui.MouseLeave += this.GuiOnMouseLeave;
            this._gui.DataContext = this;
        }

        private void GuiOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            this._gui.LapsGrid.SelectedItem = null;
        }

        private void GuiOnClosed(object sender, EventArgs eventArgs)
        {
           this.UnRegisterOnGui();
        }

        public void UnRegisterOnGui()
        {
            this._driverTiming.NewLapStarted -= this.DriverTimingOnNewLapStarted;
            this._gui.Closed -= this.GuiOnClosed;
            this._gui.MouseLeave -= this.GuiOnMouseLeave;            
            foreach (var lap in this.Laps)
            {
                lap.StopRefresh();
            }
        }

        private void DriverTimingOnNewLapStarted(object sender, DriverTiming.LapEventArgs e)
        {
            /*if (!Laps.Last().LapInfo.Completed)
            {
                Laps.RemoveAt(Laps.Count - 1);
            }*/
            var newLapModel = new LapViewModel(e.Lap);
            this.Laps.Add(newLapModel);
            this._gui.LapsGrid.ScrollIntoView(newLapModel);
        }

        public ObservableCollection<LapViewModel> Laps
        {
            get => (ObservableCollection<LapViewModel>)this.GetValue(LapsProperty);
            private set => this.SetValue(LapsProperty, value);
        }

        public string DriverName
        {
            get => (string)this.GetValue(DriverNameProperty);
            private set => this.SetValue(DriverNameProperty, value);
        }

        private void BuildLapsViewModel()
        {
            foreach (var lap in this._driverTiming.Laps)
            {
                this.Laps.Add(new LapViewModel(lap));
            }
        }
    }
}