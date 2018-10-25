namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    using DataModel.BasicProperties;

    public class SessionFuelConsumptionInfo
    {
        public SessionFuelConsumptionInfo(FuelConsumptionInfo fuelConsumptionInfo, string trackName, SessionType sessionType)
        {
            FuelConsumptionInfo = fuelConsumptionInfo;
            TrackName = trackName;
            SessionType = sessionType;
        }

        public FuelConsumptionInfo FuelConsumptionInfo { get; }
        public string TrackName { get; }
        public SessionType SessionType { get; }
    }
}
