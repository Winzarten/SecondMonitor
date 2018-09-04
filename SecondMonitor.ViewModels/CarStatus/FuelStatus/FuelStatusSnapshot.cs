namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    using System;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Systems;

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