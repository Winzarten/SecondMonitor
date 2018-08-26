namespace SecondMonitor.ViewModels.CarStatus
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.ViewModels.Annotations;

    public class CarStatusViewModel : ISimulatorDataSetViewModel, INotifyPropertyChanged
    {
        private readonly SimulatorDSViewModels _viewModels;

        private WaterTemperatureViewModel _waterTemperatureViewModel;

        private OilTemperatureViewModel _oilTemperatureViewModel;

        public CarStatusViewModel()
        {
            _viewModels = new SimulatorDSViewModels { new OilTemperatureViewModel(), new WaterTemperatureViewModel() };
            RefreshProperties();
        }

        public OilTemperatureViewModel OilTemperatureViewModel
        {
            get => _oilTemperatureViewModel;
            private set
            {
                _oilTemperatureViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public WaterTemperatureViewModel WaterTemperatureViewModel
        {
            get => _waterTemperatureViewModel;
            private set
            {
                _waterTemperatureViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
            _viewModels.ApplyDateSet(dataSet);
        }

        private void RefreshProperties()
        {
            OilTemperatureViewModel = _viewModels.GetFirst<OilTemperatureViewModel>();
            WaterTemperatureViewModel = _viewModels.GetFirst<WaterTemperatureViewModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}