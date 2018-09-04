namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    using System;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;

    public class FuelConsumptionMonitor
    {
        private FuelConsumptionInfo _totalFuelConsumption;

        private FuelStatusSnapshot _lastLapFuelStatus;

        private int _lastLapNumber;
        private FuelStatusSnapshot _lastMinuteFuelStatus;

        private FuelStatusSnapshot _lastTickFuelStatus;

        private TimeSpan _nextMinuteConsumptionUpdate;

        public FuelConsumptionMonitor()
        {
            Reset();
        }

        public Volume ActPerMinute
        {
            get;
            private set;
        }

        public Volume ActPerLap
        {
            get;
            private set;
        }

        public Volume TotalPerMinute
        {
            get;
            private set;
        }

        public Volume TotalPerLap
        {
            get;
            private set;
        }

        public void Reset()
        {
            _nextMinuteConsumptionUpdate = TimeSpan.Zero;
            _lastTickFuelStatus = null;
            _lastMinuteFuelStatus = null;
            _lastLapFuelStatus = null;
            _lastLapNumber = -1;
            _totalFuelConsumption = new FuelConsumptionInfo();
            ActPerMinute = Volume.FromLiters(0);
            ActPerLap = Volume.FromLiters(0);
            TotalPerMinute = Volume.FromLiters(0);
            TotalPerLap = Volume.FromLiters(0);
        }

        private void UpdateMinuteConsumption(SimulatorDataSet simulatorDataSet)
        {
            if (_nextMinuteConsumptionUpdate > simulatorDataSet.SessionInfo.SessionTime)
            {
                return;
            }

            _nextMinuteConsumptionUpdate = _nextMinuteConsumptionUpdate + TimeSpan.FromMinutes(1);

            if (_lastMinuteFuelStatus == null)
            {
                _lastMinuteFuelStatus = new FuelStatusSnapshot(simulatorDataSet);
                return;
            }

            FuelStatusSnapshot _currentMinuteFuelConsumption = new FuelStatusSnapshot(simulatorDataSet);
            FuelConsumptionInfo fuelConsumption = FuelConsumptionInfo.CreateConsumption(_lastMinuteFuelStatus, _currentMinuteFuelConsumption);
            ActPerMinute = fuelConsumption.ConsumedFuel;
            _lastMinuteFuelStatus = _currentMinuteFuelConsumption;
        }

        private void UpdateLapConsumption(SimulatorDataSet simulatorDataSet)
        {
            if (_lastLapNumber == simulatorDataSet.PlayerInfo.CompletedLaps)
            {
                return;
            }

            _lastLapNumber = simulatorDataSet.PlayerInfo.CompletedLaps;

            if (_lastLapFuelStatus == null)
            {
                _lastLapFuelStatus = new FuelStatusSnapshot(simulatorDataSet);
                return;
            }

            FuelStatusSnapshot _currentLapConsumption = new FuelStatusSnapshot(simulatorDataSet);
            FuelConsumptionInfo fuelConsumption = FuelConsumptionInfo.CreateConsumption(_lastLapFuelStatus, _currentLapConsumption);
            ActPerLap = fuelConsumption.ConsumedFuel;
            _lastLapFuelStatus = _currentLapConsumption;
        }

        public void UpdateFuelConsumption(SimulatorDataSet simulatorDataSet)
        {
            if (simulatorDataSet?.PlayerInfo == null)
            {
                return;
            }

            if (_lastTickFuelStatus?.SessionTime == simulatorDataSet.SessionInfo.SessionTime)
            {
                return;
            }

            if (SkipThisTick(simulatorDataSet))
            {
                // Force to also skip next tick
                _lastTickFuelStatus = null;
                return;
            }

            if (_lastTickFuelStatus == null)
            {
                _lastTickFuelStatus = new FuelStatusSnapshot(simulatorDataSet);
                return;
            }

            UpdateLapConsumption(simulatorDataSet);
            UpdateMinuteConsumption(simulatorDataSet);

            FuelStatusSnapshot currentSnapshot = new FuelStatusSnapshot(simulatorDataSet);
            FuelConsumptionInfo lastTickConsumptionInfo = FuelConsumptionInfo.CreateConsumption(_lastTickFuelStatus, currentSnapshot);

            if (!lastTickConsumptionInfo.IsFuelConsumptionValid(simulatorDataSet))
            {
                _lastTickFuelStatus = currentSnapshot;
                return;
            }

            _totalFuelConsumption = _totalFuelConsumption.AddConsumption(lastTickConsumptionInfo);
            UpdateTotalData(simulatorDataSet);
            _lastTickFuelStatus = currentSnapshot;
        }

        private void UpdateTotalData(SimulatorDataSet dataSet)
        {
            TotalPerMinute = _totalFuelConsumption.GetAveragePerMinute();
            TotalPerLap = _totalFuelConsumption.GetAveragePerDistance(dataSet.SessionInfo.TrackInfo.LayoutLength);
        }

        private bool SkipThisTick(SimulatorDataSet dataSet)
        {

            if (dataSet.PlayerInfo.InPits)
            {
                return true;
            }

            if (dataSet.SessionInfo.SessionType == SessionType.Race && dataSet.PlayerInfo.TotalDistance < 300)
            {
                return true;
            }

            if (_lastTickFuelStatus?.SessionTime > dataSet.SessionInfo.SessionTime)
            {
                return true;
            }

            return false;
        }
    }
}