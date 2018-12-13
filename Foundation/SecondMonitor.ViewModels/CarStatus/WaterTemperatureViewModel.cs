namespace SecondMonitor.ViewModels.CarStatus
{
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using Base;
    public class WaterTemperatureViewModel : AbstractTemperatureViewModel
    {
        public override Temperature MinimalTemperature { get; protected set; } = Temperature.FromCelsius(0);

        public override Temperature MaximumTemperature { get; protected set; }  = Temperature.FromCelsius(130);

        public override Temperature MaximumNormalTemperature { get; protected set; }  = Temperature.FromCelsius(110);

        protected override Temperature GetTemperatureFromDataSet(SimulatorDataSet dataSet)
        {
            if (dataSet.PlayerInfo == null)
            {
                return Temperature.Zero;
            }

            return dataSet.PlayerInfo.CarInfo.WaterSystemInfo.WaterTemperature;
        }
    }
}