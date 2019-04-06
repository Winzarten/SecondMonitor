namespace SecondMonitor.DataModel.Visitors
{
    using BasicProperties;
    using Snapshot;
    using Snapshot.Systems;

    public class ComputeSuspensionVelocityVisitor : ISimulatorDateSetVisitor
    {
        private bool _firstPackage;
        private double _lastSessionTimeSeconds;
        private Distance _lastFrontLeftSuspensionTravel;
        private Distance _lastFrontRightSuspensionTravel;
        private Distance _lastRearLeftSuspensionTravel;
        private Distance _lastRearRightSuspensionTravel;

        private Velocity _lastFrontLeftSuspensionVelocity;
        private Velocity _lastFrontRightSuspensionVelocity;
        private Velocity _lastRearLeftSuspensionVelocity;
        private Velocity _lastRearRightSuspensionVelocity;

        public ComputeSuspensionVelocityVisitor()
        {
            Reset();
        }

        public void Visit(SimulatorDataSet simulatorDataSet)
        {
            if (!simulatorDataSet.SimulatorSourceInfo.TelemetryInfo.ContainsSuspensionTravel || simulatorDataSet.SimulatorSourceInfo.TelemetryInfo.ContainsSuspensionVelocity || simulatorDataSet.PlayerInfo?.CarInfo?.WheelsInfo == null)
            {
                return;
            }

            if (!_firstPackage)
            {
                double sessionTimeSeconds = simulatorDataSet.SessionInfo.SessionTime.TotalSeconds;
                _lastFrontLeftSuspensionVelocity = ComputeSuspensionVelocity(simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.FrontLeft, _lastFrontLeftSuspensionTravel, _lastFrontLeftSuspensionVelocity, sessionTimeSeconds);
                _lastFrontRightSuspensionVelocity = ComputeSuspensionVelocity(simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.FrontRight, _lastFrontRightSuspensionTravel, _lastFrontRightSuspensionVelocity, sessionTimeSeconds);
                _lastRearLeftSuspensionVelocity = ComputeSuspensionVelocity(simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.RearLeft, _lastRearLeftSuspensionTravel, _lastRearLeftSuspensionVelocity, sessionTimeSeconds);
                _lastRearRightSuspensionVelocity = ComputeSuspensionVelocity(simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.RearRight, _lastRearRightSuspensionTravel, _lastRearRightSuspensionVelocity, sessionTimeSeconds);
                simulatorDataSet.SimulatorSourceInfo.TelemetryInfo.ContainsSuspensionVelocity = true;
            }

            _firstPackage = false;
            CaptureLastData(simulatorDataSet);
        }

        private Velocity ComputeSuspensionVelocity(WheelInfo wheel, Distance lastSuspTravel, Velocity lastSuspensionVelocity, double sessionTimeSeconds)
        {
            double interval = sessionTimeSeconds - _lastSessionTimeSeconds;
            if (interval <= 0)
            {
                wheel.SuspensionVelocity = lastSuspensionVelocity;
                return lastSuspensionVelocity;
            }

            Distance distanceTraveled = wheel.SuspensionTravel - lastSuspTravel;
            Velocity suspensionVelocity = Velocity.FromMs(distanceTraveled.InMeters / interval);
            wheel.SuspensionVelocity = suspensionVelocity;
            return suspensionVelocity;

        }

        private void CaptureLastData(SimulatorDataSet simulatorDataSet)
        {
            _lastSessionTimeSeconds = simulatorDataSet.SessionInfo.SessionTime.TotalSeconds;
            _lastFrontLeftSuspensionTravel = simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.SuspensionTravel;
            _lastFrontRightSuspensionTravel = simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.FrontRight.SuspensionTravel;
            _lastRearLeftSuspensionTravel = simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.RearLeft.SuspensionTravel;
            _lastRearRightSuspensionTravel = simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.RearRight.SuspensionTravel;
        }

        public void Reset()
        {
            _firstPackage = true;
            _lastFrontLeftSuspensionVelocity = Velocity.Zero;
            _lastFrontRightSuspensionVelocity = Velocity.Zero;
            _lastRearLeftSuspensionVelocity = Velocity.Zero;
            _lastRearRightSuspensionVelocity = Velocity.Zero;
        }
    }
}