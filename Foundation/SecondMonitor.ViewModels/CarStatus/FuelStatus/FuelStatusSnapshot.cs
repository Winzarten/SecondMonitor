namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    using System;

    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Systems;

    public class FuelStatusSnapshot
    {

        public FuelStatusSnapshot(SimulatorDataSet dataSet)
        {
            FuelInfo fuelInfo = dataSet.PlayerInfo.CarInfo.FuelSystemInfo;
            FuelLevel = fuelInfo.FuelRemaining;
            SessionTime = dataSet.SessionInfo.SessionTime;
            TotalDistance = dataSet.PlayerInfo.TotalDistance;
        }

        public Volume FuelLevel { get; }
        public TimeSpan SessionTime { get; }
        public double TotalDistance { get; }

    }
}       