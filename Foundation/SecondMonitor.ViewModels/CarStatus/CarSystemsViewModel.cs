namespace SecondMonitor.ViewModels.CarStatus
{
    using System.Diagnostics;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;

    public class CarSystemsViewModel : AbstractViewModel, ISimulatorDataSetViewModel
    {

        private OptimalQuantity<Temperature> _waterTemperature;
        private OptimalQuantity<Temperature> _oilTemperature;
        private Pressure _turboPressure;
        private Pressure _oilPressure;
        private Pressure _waterPressure;
        private Pressure _fuelPressure;

        public CarSystemsViewModel()
        {
            TurboPressure = Pressure.Zero;
        }

        public Pressure OilPressure
        {
            get => _oilPressure;
            set => SetProperty(ref _oilPressure, value);
        }

        public Pressure WaterPressure
        {
            get => _waterPressure;
            set => SetProperty(ref _waterPressure, value);
        }

        public Pressure FuelPressure
        {
            get => _fuelPressure;
            set => SetProperty(ref _fuelPressure, value);
        }

        public Pressure TurboPressure
        {
            get => _turboPressure;
            set => SetProperty(ref _turboPressure, value);
        }

        public OptimalQuantity<Temperature> WaterTemperature
        {
            get => _waterTemperature;
            set => SetProperty(ref _waterTemperature, value);
        }

        public OptimalQuantity<Temperature> OilTemperature
        {
            get => _oilTemperature;
            set => SetProperty(ref _oilTemperature, value);
        }

        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
            if (dataSet?.PlayerInfo == null)
            {
                return;
            }

            WaterTemperature = dataSet.PlayerInfo.CarInfo.WaterSystemInfo.OptimalWaterTemperature;
            OilTemperature = dataSet.PlayerInfo.CarInfo.OilSystemInfo.OptimalOilTemperature;
            TurboPressure = dataSet.PlayerInfo.CarInfo.TurboPressure;
            OilPressure = dataSet.PlayerInfo.CarInfo.OilSystemInfo.OilPressure;
            WaterPressure = dataSet.PlayerInfo.CarInfo.WaterSystemInfo.WaterPressure;
            FuelPressure = dataSet.PlayerInfo.CarInfo.FuelSystemInfo.FuelPressure;
        }

        public void Reset()
        {

        }
    }
}