namespace SecondMonitor.Timing.LapTimings.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using SecondMonitor.Timing.SessionTiming.Drivers.ViewModel;

    using DriverLapsWindow = View.DriverLapsWindow;

    public class DriverLapsViewModel : DependencyObject
    {

        public static readonly DependencyProperty LapsProperty = DependencyProperty.Register("Laps", typeof(ObservableCollection<LapViewModel>), typeof(DriverLapsViewModel));
        public static readonly DependencyProperty DriverNameProperty = DependencyProperty.Register("DriverName", typeof(string), typeof(DriverLapsViewModel));


        private readonly DriverTiming _driverTiming;

        private readonly DriverLapsWindow _gui;

        public DriverLapsViewModel()
        {
            DriverName = "Design Time";
            Laps = new ObservableCollection<LapViewModel>();
        }

        public DriverTiming DriverTiming => _driverTiming;

        public DriverLapsViewModel(DriverTiming driverTiming, DriverLapsWindow gui)
        {
            _driverTiming = driverTiming;
            Laps = new ObservableCollection<LapViewModel>();
            BuildLapsViewModel();
            _driverTiming.NewLapStarted += DriverTimingOnNewLapStarted;
            DriverName = _driverTiming.Name;
            _gui = gui;
            _gui.Closed += GuiOnClosed;
            _gui.MouseLeave += GuiOnMouseLeave;
            _gui.DataContext = this;
        }

        private void GuiOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            _gui.LapsGrid.SelectedItem = null;
        }

        private void GuiOnClosed(object sender, EventArgs eventArgs)
        {
            UnRegisterOnGui();
        }

        public void UnRegisterOnGui()
        {
            _driverTiming.NewLapStarted -= DriverTimingOnNewLapStarted;
            _gui.Closed -= GuiOnClosed;
            _gui.MouseLeave -= GuiOnMouseLeave;
            foreach (var lap in Laps)
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
            if (Laps.Any() && Laps.Last().LapNumber == newLapModel.LapNumber)
            {
                Laps.Remove(Laps.Last());
            }
            Laps.Add(newLapModel);
            _gui.LapsGrid.ScrollIntoView(newLapModel);
        }

        public ObservableCollection<LapViewModel> Laps
        {
            get => (ObservableCollection<LapViewModel>)GetValue(LapsProperty);
            private set => SetValue(LapsProperty, value);
        }

        public string DriverName
        {
            get => (string)GetValue(DriverNameProperty);
            private set => SetValue(DriverNameProperty, value);
        }

        private void BuildLapsViewModel()
        {
            foreach (var lap in _driverTiming.Laps)
            {
                if (Laps.Any() && Laps.Last().LapNumber == lap.LapNumber)
                {
                    Laps.Remove(Laps.Last());
                }

                Laps.Add(new LapViewModel(lap));
            }
        }
    }
}