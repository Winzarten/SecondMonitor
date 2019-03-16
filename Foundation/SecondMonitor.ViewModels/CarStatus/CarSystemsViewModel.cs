namespace SecondMonitor.ViewModels.CarStatus
{
    using System.Diagnostics;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;

    public class CarSystemsViewModel : AbstractViewModel, ISimulatorDataSetViewModel
    {

        private readonly Stopwatch _refreshStopwatch;

        private Temperature _waterTemperature;
        private Temperature _oilTemperature;
        private Pressure _turboPressure;
        private Pressure _oilPressure;
        private Pressure _waterPressure;
        private Pressure _fuelPressure;

        public CarSystemsViewModel()
        {
            _refreshStopwatch = Stopwatch.StartNew();
            WaterTemperature = Temperature.Zero;
            OilTemperature = Temperature.Zero;
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

        public Temperature WaterTemperature
        {
            get => _waterTemperature;
            set => SetProperty(ref _waterTemperature, value);
        }

        public Temperature OilTemperature
        {
            get => _oilTemperature;
            set => SetProperty(ref _oilTemperature, value);
        }

        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
            if (dataSet?.PlayerInfo == null || _refreshStopwatch.ElapsedMilliseconds < 100)
            {
                return;
            }

            WaterTemperature = dataSet.PlayerInfo.CarInfo.WaterSystemInfo.WaterTemperature;
            WaterPressure = dataSet.PlayerInfo.CarInfo.WaterSystemInfo.WaterPressure;
            OilTemperature = dataSet.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature;
            OilPressure = dataSet.PlayerInfo.CarInfo.OilSystemInfo.OilPressure;
            TurboPressure = dataSet.PlayerInfo.CarInfo.TurboPressure;
            FuelPressure = dataSet.PlayerInfo.CarInfo.FuelSystemInfo.FuelPressure;
            _refreshStopwatch.Restart();

        }

        public void Reset()
        {

        }
    }
}