namespace SecondMonitor.WindowsControls.WPF.DriverPosition
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using DataModel.BasicProperties;
    using DataModel.Extensions;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;

    public abstract class AbstractSituationOverviewControl : Grid, ISituationOverviewControl
    {
        private static readonly DependencyProperty PlayerForegroundBrushProperty = DependencyProperty.Register("PlayerForegroundBrush", typeof(SolidColorBrush), typeof(AbstractSituationOverviewControl));
        private static readonly DependencyProperty PlayerBackgroundBrushProperty = DependencyProperty.Register("PlayerBackgroundBrush", typeof(SolidColorBrush), typeof(AbstractSituationOverviewControl));
        private static readonly DependencyProperty DriverForegroundBrushProperty = DependencyProperty.Register("DriverForegroundBrush", typeof(SolidColorBrush), typeof(AbstractSituationOverviewControl));
        private static readonly DependencyProperty DriverBackgroundBrushProperty = DependencyProperty.Register("DriverBackgroundBrush", typeof(SolidColorBrush), typeof(AbstractSituationOverviewControl));
        private static readonly DependencyProperty LappedDriverForegroundBrushProperty = DependencyProperty.Register("LappedDriverForegroundBrush", typeof(SolidColorBrush), typeof(AbstractSituationOverviewControl));
        private static readonly DependencyProperty LappedDriverBackgroundBrushProperty = DependencyProperty.Register("LappedDriverBackgroundBrush", typeof(SolidColorBrush), typeof(AbstractSituationOverviewControl));
        private static readonly DependencyProperty LappingDriverForegroundBrushProperty = DependencyProperty.Register("LappingDriverForegroundBrush", typeof(SolidColorBrush), typeof(AbstractSituationOverviewControl));
        private static readonly DependencyProperty LappingDriverBackgroundBrushProperty = DependencyProperty.Register("LappingDriverBackgroundBrush", typeof(SolidColorBrush), typeof(AbstractSituationOverviewControl));
        private static readonly DependencyProperty DriverPitsForegroundBrushProperty = DependencyProperty.Register("DriverPitsForegroundBrush", typeof(SolidColorBrush), typeof(AbstractSituationOverviewControl));
        private static readonly DependencyProperty DriverPitsBackgroundBrushProperty = DependencyProperty.Register("DriverPitsBackgroundBrush", typeof(SolidColorBrush), typeof(AbstractSituationOverviewControl));
        private static readonly DependencyProperty AdditionalInformationProperty = DependencyProperty.Register("AdditionalInformation", typeof(string), typeof(AbstractSituationOverviewControl));
        public static readonly DependencyProperty PlayerOutLineBrushProperty = DependencyProperty.Register("PlayerOutLineBrush", typeof(SolidColorBrush), typeof(AbstractSituationOverviewControl), new PropertyMetadata(Brushes.Transparent));

        private readonly Dictionary<string, DriverPositionControl> _drivers;

        private bool _animateDriversPosition;

        protected AbstractSituationOverviewControl()
        {
            _drivers = new Dictionary<string, DriverPositionControl>();
        }

        public SolidColorBrush PlayerOutLineBrush
        {
            get => (SolidColorBrush) GetValue(PlayerOutLineBrushProperty);
            set => SetValue(PlayerOutLineBrushProperty, value);
        }

        public string AdditionalInformation
        {
            get => (string)GetValue(AdditionalInformationProperty);
            set => SetValue(AdditionalInformationProperty, value);
        }

        public SolidColorBrush PlayerForegroundBrush
        {
            get => (SolidColorBrush)GetValue(PlayerForegroundBrushProperty);
            set => SetValue(PlayerForegroundBrushProperty, value);
        }

        public SolidColorBrush PlayerBackgroundBrush
        {
            get => (SolidColorBrush)GetValue(PlayerBackgroundBrushProperty);
            set => SetValue(PlayerBackgroundBrushProperty, value);
        }

        public SolidColorBrush DriverForegroundBrush
        {
            get => (SolidColorBrush)GetValue(DriverForegroundBrushProperty);
            set => SetValue(DriverForegroundBrushProperty, value);
        }

        public SolidColorBrush DriverBackgroundBrush
        {
            get => (SolidColorBrush)GetValue(DriverBackgroundBrushProperty);
            set => SetValue(DriverBackgroundBrushProperty, value);
        }

        public SolidColorBrush DriverPitsForegroundBrush
        {
            get => (SolidColorBrush)GetValue(DriverPitsForegroundBrushProperty);
            set => SetValue(DriverPitsForegroundBrushProperty, value);
        }

        public SolidColorBrush DriverPitsBackgroundBrush
        {
            get => (SolidColorBrush)GetValue(DriverPitsBackgroundBrushProperty);
            set => SetValue(DriverPitsBackgroundBrushProperty, value);
        }

        public SolidColorBrush LappedDriverForegroundBrush
        {
            get => (SolidColorBrush)GetValue(LappedDriverForegroundBrushProperty);
            set => SetValue(LappedDriverForegroundBrushProperty, value);
        }

        public SolidColorBrush LappedDriverBackgroundBrush
        {
            get => (SolidColorBrush)GetValue(LappedDriverBackgroundBrushProperty);
            set => SetValue(LappedDriverBackgroundBrushProperty, value);
        }

        public SolidColorBrush LappingDriverForegroundBrush
        {
            get => (SolidColorBrush)GetValue(LappingDriverForegroundBrushProperty);
            set => SetValue(LappingDriverForegroundBrushProperty, value);
        }

        public SolidColorBrush LappingDriverBackgroundBrush
        {
            get => (SolidColorBrush)GetValue(LappingDriverBackgroundBrushProperty);
            set => SetValue(LappingDriverBackgroundBrushProperty, value);
        }

        public bool AnimateDriversPos
        {
            get => _animateDriversPosition;
            set
            {
                _animateDriversPosition = value;
                _drivers.Values.ForEach(x => x.Animate = value);
            }
        }


        public IPositionCircleInformationProvider PositionCircleInformationProvider
        {
            get;
            set;
        }

        protected SimulatorDataSet LastDataSet { get; set; }

        public void AddDrivers(params IDriverInfo[] drivers)
        {

        }

        public void RemoveDrivers(params IDriverInfo[] drivers)
        {
            foreach (IDriverInfo driver in drivers)
            {
                RemoveDriver(driver.DriverName);
            }
        }

        public virtual void UpdateDrivers(SimulatorDataSet dataSet, params IDriverInfo[] drivers)
        {
            LastDataSet = dataSet;
            foreach (IDriverInfo driver in drivers)
            {
                if (_drivers.ContainsKey(driver.DriverName))
                {
                    UpdateDriver(driver, _drivers[driver.DriverName]);
                }
                else
                {
                    AddDriver(driver);
                }
            }
        }

        public void RemoveAllDrivers()
        {
            foreach (DriverPositionControl control in _drivers.Values)
            {
                RemoveDriver(control);
            }

            _drivers.Clear();
        }



        private void AddDriver(IDriverInfo driverInfo)
        {
            lock (_drivers)
            {
                if (_drivers.ContainsKey(driverInfo.DriverName))
                {
                    RemoveDriver(driverInfo.DriverName);
                }

                DriverPositionControl newDriverControl =
                    new DriverPositionControl
                    {
                        Width = GetDriverControlSize(),
                        Height = GetDriverControlSize(),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Animate = AnimateDriversPos,
                    };
                PostDriverCreation(newDriverControl);
                UpdateDriver(driverInfo, newDriverControl);
                AddDriver(newDriverControl);
                _drivers[driverInfo.DriverName] = newDriverControl;
            }
        }

        protected abstract void PostDriverCreation(DriverPositionControl driverPositionControl);

        protected abstract void RemoveDriver(DriverPositionControl driverPositionControl);

        protected abstract void AddDriver(DriverPositionControl driverPositionControl);

        protected abstract double GetDriverControlSize();

        private void RemoveDriver(string driverName)
        {
            if (!_drivers.ContainsKey(driverName))
            {
                return;
            }

            DriverPositionControl driverToRemove = _drivers[driverName];
            RemoveDriver(driverToRemove);
            _drivers.Remove(driverName);
        }

        private void UpdateDriver(IDriverInfo driverInfo, DriverPositionControl driverPositionControl)
        {
            driverPositionControl.Position = driverInfo.Position;
            driverPositionControl.X = GetX(driverInfo);
            driverPositionControl.Y = GetY(driverInfo);
            UpdateColors(driverInfo, driverPositionControl);
        }

        private void UpdateColors(IDriverInfo driverInfo, DriverPositionControl driverPositionControl)
        {
            if (driverInfo.IsPlayer)
            {
                driverPositionControl.CircleBrush = PlayerBackgroundBrush;
                driverPositionControl.TextBrush = PlayerForegroundBrush;
                SetZIndex(driverPositionControl, 100);
                driverPositionControl.OutLineColor = PlayerOutLineBrush;
                return;
            }

            if(PositionCircleInformationProvider.GetTryCustomOutline(driverInfo, out SolidColorBrush outlineBrush))
            {
                driverPositionControl.OutLineColor = outlineBrush;
                SetZIndex(driverPositionControl, 100);
            }

            if (driverInfo.InPits)
            {
                driverPositionControl.CircleBrush = DriverPitsBackgroundBrush;
                driverPositionControl.TextBrush = DriverPitsForegroundBrush;
                return;
            }

            if (IsLapped(driverInfo))
            {
                driverPositionControl.CircleBrush = LappedDriverBackgroundBrush;
                driverPositionControl.TextBrush = LappedDriverForegroundBrush;
                return;
            }

            if (driverInfo.IsLappingPlayer)
            {
                driverPositionControl.CircleBrush = LappingDriverBackgroundBrush;
                driverPositionControl.TextBrush = LappingDriverForegroundBrush;
                return;
            }

            driverPositionControl.CircleBrush = DriverBackgroundBrush;
            driverPositionControl.TextBrush = DriverForegroundBrush;
        }

        private bool IsLapped(IDriverInfo driver)
        {
            if (LastDataSet?.SessionInfo.SessionType == SessionType.Race || PositionCircleInformationProvider == null)
            {
                return driver.IsBeingLappedByPlayer;
            }

            return !PositionCircleInformationProvider.IsDriverOnValidLap(driver);
        }

        protected abstract double GetLabelSize();
        protected abstract double GetX(IDriverInfo driver);
        protected abstract double GetY(IDriverInfo driver);
    }
}