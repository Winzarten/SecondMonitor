namespace SecondMonitor.ViewModels.SituationOverview
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media;

    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;
    using SecondMonitor.ViewModels.Annotations;
    using SecondMonitor.WindowsControls.WPF;
    using SecondMonitor.WindowsControls.WPF.DriverPosition;
    using SecondMonitor.WindowsControls.WPF.DriverPostion;

    public class SituationOverviewProvider : ISimulatorDataSetViewModel, INotifyPropertyChanged
    {

        private readonly ResourceDictionary _commonResources;

        private PositionCircleControl _positionCircle;

        private ISituationOverviewControl _situationOverviewControl;

        private IPositionCircleInformationProvider _positionCircleInformationProvider;

        public SituationOverviewProvider(IPositionCircleInformationProvider positionCircleInformation)
        {
            _commonResources = new ResourceDictionary
                                   {
                                       Source = new Uri(
                                           @"pack://application:,,,/WindowsControls;component/WPF/CommonResources.xaml",
                                           UriKind.RelativeOrAbsolute)
                                   };

            PositionCircleInformationProvider = positionCircleInformation;
            InitializePositionCircle();
            SituationOverviewControl = _positionCircle;

        }

        public ISituationOverviewControl SituationOverviewControl
        {
            get => _situationOverviewControl;
            private set
            {
                _situationOverviewControl = value;
                NotifyPropertyChanged();
            }
        }

        public IPositionCircleInformationProvider PositionCircleInformationProvider
        {
            get => _positionCircleInformationProvider;

            set
            {
                _positionCircleInformationProvider = value;
                if(_positionCircle != null)
                {
                    _positionCircle.PositionCircleInformationProvider = value;
                }
            }
        }

        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
            _positionCircle.UpdateDrivers(dataSet, dataSet.DriversInfo);
        }

        public void Reset()
        {
            _situationOverviewControl?.RemoveAllDrivers();
        }

        public void RemoveDriver(DriverInfo driver)
        {
            _situationOverviewControl.RemoveDrivers(driver);
        }

        public void AddDriver(DriverInfo driver)
        {
            _situationOverviewControl.AddDrivers(driver);
        }

        private void InitializePositionCircle()
        {
            _positionCircle = new PositionCircleControl
            {
                PositionCircleInformationProvider = PositionCircleInformationProvider,

                DriverBackgroundBrush = (SolidColorBrush)_commonResources["DriverBackgroundColor"],
                DriverForegroundBrush = (SolidColorBrush)_commonResources["DriverForegroundColor"],

                DriverPitsBackgroundBrush = (SolidColorBrush)_commonResources["DriverPitsBackgroundColor"],
                DriverPitsForegroundBrush = (SolidColorBrush)_commonResources["DriverPitsForegroundColor"],

                PlayerBackgroundBrush = (SolidColorBrush)_commonResources["PlayerBackgroundColor"],
                PlayerForegroundBrush = (SolidColorBrush)_commonResources["PlayerForegroundColor"],

                LappedDriverBackgroundBrush = (SolidColorBrush)_commonResources["TimingLappedBrush"],
                LappedDriverForegroundBrush = (SolidColorBrush)_commonResources["TimingLappedForegroundBrush"],

                LappingDriverBackgroundBrush = (SolidColorBrush)_commonResources["TimingLappingBrush"],
                LappingDriverForegroundBrush = (SolidColorBrush)_commonResources["TimingLappingForegroundBrush"]
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}