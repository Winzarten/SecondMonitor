namespace SecondMonitor.PluginManager.GameConnector
{
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;

    public abstract class AbstractDataConvertor
    {
        protected static void ComputeDistanceToPlayer(DriverInfo player, DriverInfo driverInfo, double trackLength)
        {
            if (player == null)
            {
                return;
            }

            if (driverInfo.FinishStatus == DriverFinishStatus.Dq || driverInfo.FinishStatus == DriverFinishStatus.Dnf ||
                driverInfo.FinishStatus == DriverFinishStatus.Dnq || driverInfo.FinishStatus == DriverFinishStatus.Dns)
            {
                driverInfo.DistanceToPlayer = double.MaxValue;
                return;
            }

            double playerLapDistance = player.LapDistance;

            double distanceToPlayer = playerLapDistance - driverInfo.LapDistance;
            if (distanceToPlayer < -(trackLength / 2))
            {
                distanceToPlayer = distanceToPlayer + trackLength;
            }

            if (distanceToPlayer > (trackLength / 2))
            {
                distanceToPlayer = distanceToPlayer - trackLength;
            }

            driverInfo.DistanceToPlayer = distanceToPlayer;
        }

        protected static void PopulateClassPositions(SimulatorDataSet dataSet)
        {
            List<IGrouping<string, DriverInfo>> classPotions = dataSet.DriversInfo.GroupBy(x => x.CarClassId).ToList();

            dataSet.SessionInfo.IsMultiClass = classPotions.Count > 1;

            foreach (IGrouping<string, DriverInfo> classPotion in classPotions)
            {
                int position = 1;
                foreach (DriverInfo driverInfo in classPotion.OrderBy(x => x.Position))
                {
                    driverInfo.PositionInClass = position;
                    position++;
                }
            }
        }
    }
}