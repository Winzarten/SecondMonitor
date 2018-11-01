namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    using Contracts.FuelInformation;
    using DataModel.BasicProperties;

    public class SessionFuelConsumptionInfo
    {
        public SessionFuelConsumptionInfo(IFuelConsumptionInfo fuelConsumptionInfo, string trackName, Distance lapDistance, SessionType sessionType)
        {
            FuelConsumptionInfo = fuelConsumptionInfo;
            TrackName = trackName;
            LapDistance = lapDistance;
            SessionType = sessionType;
        }

        public IFuelConsumptionInfo FuelConsumptionInfo { get; }
        public string TrackName { get; }
        public Distance LapDistance { get; }
        public SessionType SessionType { get; }
    }
}
