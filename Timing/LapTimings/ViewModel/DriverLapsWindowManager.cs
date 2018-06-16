namespace SecondMonitor.Timing.LapTimings.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    using SecondMonitor.Timing.LapTimings.View;
    using SecondMonitor.Timing.Presentation.ViewModel.Commands;
    using SecondMonitor.Timing.SessionTiming.Drivers.ModelView;

    public class DriverLapsWindowManager
    {
        private readonly List<DriverLapsWindow> _openedWindows = new List<DriverLapsWindow>();

        public DriverLapsWindowManager(Func<Window> getWindowOwnerFunc, Func<DriverTiming> getDriverTiming)
        {
            GetDriverTiming = getDriverTiming;
            GetWindowOwnerFunc = getWindowOwnerFunc;
        }

        public Func<Window> GetWindowOwnerFunc
        {
            get;
            set;
        }

        public Func<DriverTiming> GetDriverTiming
        {
            get;
            set;
        }

        public ICommand OpenWindowCommand => new NoArgumentCommand(OpenWindowDefault);

        public void OpenWindowDefault()
        {
            OpenWindow(GetDriverTiming(), GetWindowOwnerFunc());
        }

        private void OpenWindow(DriverTiming driverTiming, Window ownerWindow )
        {            
            DriverLapsWindow lapsWindow = new DriverLapsWindow()
                                              {
                                                  Owner = ownerWindow,
                                                  WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                              };
            new DriverLapsViewModel(driverTiming, lapsWindow);
            _openedWindows.Add(lapsWindow);
            lapsWindow.Closed += LapsWindow_Closed;
            lapsWindow.Show();
        }

        private void LapsWindow_Closed(object sender, EventArgs e)
        {
            if (sender is DriverLapsWindow window)
            {
                _openedWindows.Remove(window);
            }
        }

        public void Rebind(DriverTiming driverTiming)
        {
            _openedWindows.FindAll(p => ((DriverLapsViewModel)p.DataContext).DriverTiming.Name == driverTiming.Name).ForEach(p => Rebind(p, driverTiming));
        }

        private void Rebind(DriverLapsWindow window, DriverTiming newViewModel)
        {
            if (!(window.DataContext is DriverLapsViewModel oldDriverLapsViewModel))
            {
                return;
            }
            oldDriverLapsViewModel.UnRegisterOnGui();
            window.DataContext = new DriverLapsViewModel(newViewModel, window);
        }
    }
}