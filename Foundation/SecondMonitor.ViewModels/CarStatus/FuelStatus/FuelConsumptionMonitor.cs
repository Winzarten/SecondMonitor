using System.Collections.Generic;

namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    using System;
    using Contracts.FuelInformation;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;

    public class FuelConsumptionMonitor
    {
        private static readonly TimeSpan MinimumSessionLength  = TimeSpan.FromMinutes(2);

        private readonly List<SessionFuelConsumptionInfo> _sessionsFuelConsumptionInfos;
        private IFuelConsumptionInfo _totalFuelConsumption;
        private FuelStatusSnapshot _lastLapFuelStatus;
        private int _lastLapNumber;
        private FuelStatusSnapshot _lastMinuteFuelStatus;
        private FuelStatusSnapshot _lastTickFuelStatus;
        private TimeSpan _nextMinuteConsumptionUpdate;
        private SimulatorDataSet _lastDataSet;

        public FuelConsumptionMonitor()
        {
            _sessionsFuelConsumptionInfos = new List<SessionFuelConsumptionInfo>();
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

        public IReadOnlyCollection<SessionFuelConsumptionInfo> SessionFuelConsumptionInfos
        {
            get
            {
                List<SessionFuelConsumptionInfo> items = new List<SessionFuelConsumptionInfo>();
                items.AddRange(CompletedSessionsConsumptions);

                items.Add(GetConsumptionForCurrentSession());
                items.Reverse();
                return items.AsReadOnly();
            }
        }

        public IReadOnlyCollection<SessionFuelConsumptionInfo> CompletedSessionsConsumptions =>
            _sessionsFuelConsumptionInfos.AsReadOnly();

        public void Reset()
        {
            CreateAndAddSessionFuelConsumptionInfo();
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

        private void CreateAndAddSessionFuelConsumptionInfo()
        {
            if (_lastDataSet == null || _totalFuelConsumption.ElapsedTime < MinimumSessionLength)
            {
                return;
            }

            _sessionsFuelConsumptionInfos.Add(GetConsumptionForCurrentSession());
        }

        private SessionFuelConsumptionInfo GetConsumptionForCurrentSession()
        {
            if (_lastDataSet == null || _totalFuelConsumption.ElapsedTime < MinimumSessionLength)
            {
                return null;
            }

            return new SessionFuelConsumptionInfo(_totalFuelConsumption,
                _lastDataSet.SessionInfo.TrackInfo.TrackName, Distance.FromMeters(_lastDataSet.SessionInfo.TrackInfo.LayoutLength.InMeters), _lastDataSet.SessionInfo.SessionType);
        }

        private void UpdateMinuteConsumption(SimulatorDataSet simulatorDataSet)
        {
            if (_nextMinuteConsumptionUpdate > simulatorDataSet.SessionInfo.SessionTime)
            {
                return;
            }

            _lastDataSet = simulatorDataSet;
            _nextMinuteConsumptionUpdate = _nextMinuteConsumptionUpdate + TimeSpan.FromMinutes(1);

            if (_lastMinuteFuelStatus == null)
            {
                _lastMinuteFuelStatus = new FuelStatusSnapshot(simulatorDataSet);
                return;
            }

            FuelStatusSnapshot currentMinuteFuelConsumption = new FuelStatusSnapshot(simulatorDataSet);
            FuelConsumptionInfo fuelConsumption = FuelConsumptionInfo.CreateConsumption(_lastMinuteFuelStatus, currentMinuteFuelConsumption);
            ActPerMinute = fuelConsumption.ConsumedFuel;
            _lastMinuteFuelStatus = currentMinuteFuelConsumption;
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

            FuelStatusSnapshot currentLapConsumption = new FuelStatusSnapshot(simulatorDataSet);
            FuelConsumptionInfo fuelConsumption = FuelConsumptionInfo.CreateConsumption(_lastLapFuelStatus, currentLapConsumption);
            ActPerLap = fuelConsumption.ConsumedFuel;
            _lastLapFuelStatus = currentLapConsumption;
        }

        public void UpdateFuelConsumption(SimulatorDataSet simulatorDataSet)
        {
            if (simulatorDataSet?.PlayerInfo == null)
            {
                return;
            }

            if (simulatorDataSet?.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining.InLiters <= 0)
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