namespace SecondMonitor.ViewModels.CarStatus
{
    using SecondMonitor.DataModel.Snapshot;

    public class CarStatusViewModel : ISimulatorDataSetViewModel
    {
        private readonly SimulatorDSViewModels _viewModels;

        public CarStatusViewModel()
        {
            _viewModels = new SimulatorDSViewModels { new OilTemperatureViewModel(), new WaterTemperatureViewModel() };
            RefreshProperties();
        }

        public OilTemperatureViewModel OilTemperatureViewModel
        {
            get;
            private set;
        }

        public WaterTemperatureViewModel WaterTemperatureViewModel
        {
            get;
            private set;
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
    }
}