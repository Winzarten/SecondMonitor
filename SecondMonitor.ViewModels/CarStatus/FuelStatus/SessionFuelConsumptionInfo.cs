namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    using DataModel.BasicProperties;

    public class SessionFuelConsumptionInfo
    {
        public SessionFuelConsumptionInfo(FuelConsumptionInfo fuelConsumptionInfo, string trackName, Distance lapDistance, SessionType sessionType)
        {
            FuelConsumptionInfo = fuelConsumptionInfo;
            TrackName = trackName;
            LapDistance = lapDistance;
            SessionType = sessionType;
        }

        public FuelConsumptionInfo FuelConsumptionInfo { get; }
        public string TrackName { get; }
        public Distance LapDistance { get; }
        public SessionType SessionType { get; }
    }
}
