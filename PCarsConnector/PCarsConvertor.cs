namespace SecondMonitor.PCarsConnector
{
    using System;
    using System.Collections.Generic;

    using SecondMonitor.DataModel;
    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;
    using SecondMonitor.PCarsConnector.enums;

    public class PCarsConvertor
    {

        private readonly TimeSpan _pitTimeDelay = TimeSpan.FromMilliseconds(2000);
        private readonly int _pitRaceTimeCheckDelay = 20000;
        private readonly Dictionary<string, TimeSpan> _pitTriggerTimes;
        private readonly HashSet<string> _driversInPits;
        private readonly PCarsConnector _pcarsConnector;
        private SimulatorDataSet _lastSpeedComputationSet;
        private DriverInfo _lastPlayer = new DriverInfo();

        public PCarsConvertor(PCarsConnector pcarsConnector)
        {
            _pcarsConnector = pcarsConnector;
            _pitTriggerTimes = new Dictionary<string, TimeSpan>();
            _driversInPits = new HashSet<string>();
        }

        public Dictionary<string, TimeSpan> PitTriggerTimes => _pitTriggerTimes;

        public HashSet<string> DriversInPits => _driversInPits;

        private static void ComputeDistanceToPlayer(DriverInfo player, DriverInfo driverInfo, SimulatorDataSet data)
        {
            double trackLength = data.SessionInfo.TrackInfo.LayoutLength;
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

        private void AddDriversData(PCarsApiStruct pcarsData, SimulatorDataSet data)
        {
            if (pcarsData.MNumParticipants == -1)
            {
                return;
            }

            data.DriversInfo = new DriverInfo[pcarsData.MNumParticipants];
            DriverInfo playersInfo = null;
            var computeSpeed = data.SessionInfo.SessionTime.TotalSeconds > _pcarsConnector.NextSpeedComputation;
            for (int i = 0; i < pcarsData.MNumParticipants; i++)
            {
                PCarsApiParticipantStruct pcarsDriverData = pcarsData.MParticipantData[i];
                DriverInfo driverInfo =
                    new DriverInfo
                    {
                        DriverName = pcarsDriverData.MName,
                        CompletedLaps = (int)pcarsDriverData.MLapsCompleted,
                        CarName = pcarsData.MCarClassName,
                        InPits = false,
                        IsPlayer = i == pcarsData.MViewedParticipantIndex,
                        Position = (int)pcarsDriverData.MRacePosition,
                        LapDistance = pcarsDriverData.MCurrentLapDistance,
                        FinishStatus = DriverInfo.DriverFinishStatus.None,
                        CurrentLapValid = true,
                        WorldPosition = new Point3D(pcarsDriverData.MWorldPosition[0], pcarsDriverData.MWorldPosition[1], pcarsDriverData.MWorldPosition[2])
                    };

                if (_lastPlayer != null)
                {
                    ComputeDistanceToPlayer(_lastPlayer, driverInfo, data);
                }

                if (pcarsData.MLapsInEvent > 0 && driverInfo.CompletedLaps >= pcarsData.MLapsInEvent)
                {
                    driverInfo.FinishStatus = DriverInfo.DriverFinishStatus.Finished;
                }

                driverInfo.TotalDistance = driverInfo.CompletedLaps * data.SessionInfo.TrackInfo.LayoutLength + driverInfo.LapDistance;

                if (driverInfo.IsPlayer)
                {
                    playersInfo = driverInfo;
                    driverInfo.CurrentLapValid = !pcarsData.MLapInvalidated;
                }
                else
                {
                    driverInfo.CurrentLapValid = true;
                }

                data.DriversInfo[i] = driverInfo;
                if (string.IsNullOrEmpty(driverInfo.DriverName))
                {
                    throw new PCarsConnector.NameNotFilledException("Name not filled for driver with index " + i);
                }

                AddSpeedInfo(data, computeSpeed, driverInfo);
                if (data.SessionInfo.SessionType == SessionType.Race && _lastPlayer != null && _lastPlayer.CompletedLaps != 0)
                {
                    driverInfo.IsBeingLappedByPlayer = driverInfo.TotalDistance < (_lastPlayer.TotalDistance - data.SessionInfo.TrackInfo.LayoutLength * 0.5);
                    driverInfo.IsLappingPlayer = _lastPlayer.TotalDistance < (driverInfo.TotalDistance - data.SessionInfo.TrackInfo.LayoutLength * 0.5);
                }

                if (driverInfo.Position == 1)
                {
                    data.LeaderInfo = driverInfo;
                    data.SessionInfo.LeaderCurrentLap = driverInfo.CompletedLaps + 1;
                }
            }

            if (computeSpeed)
            {
                _pcarsConnector.NextSpeedComputation += 0.5;
            }

            if (playersInfo != null)
            {
                data.PlayerInfo = playersInfo;
                _lastPlayer = playersInfo;
            }

            AddPitsInfo(data);
            if (computeSpeed)
            {
                _lastSpeedComputationSet = data;
            }

        }

        private void AddSpeedInfo(SimulatorDataSet data, bool computeNewSpeed, DriverInfo driverInfo)
        {
            if (!computeNewSpeed && _pcarsConnector.PreviousTickInfo.ContainsKey(driverInfo.DriverName))
            {
                driverInfo.Speed = _pcarsConnector.PreviousTickInfo[driverInfo.DriverName].Speed;
            }

            if (_pcarsConnector.PreviousTickInfo.ContainsKey(driverInfo.DriverName) && computeNewSpeed && _lastSpeedComputationSet != null)
            {
                Point3D currentWorldPosition = driverInfo.WorldPosition;
                Point3D previousWorldPosition = _pcarsConnector.PreviousTickInfo[driverInfo.DriverName].WorldPosition;
                double duration = data.SessionInfo.SessionTime
                    .Subtract(_lastSpeedComputationSet.SessionInfo.SessionTime).TotalSeconds;

                // double speed = lastTickDuration.TotalMilliseconds;
                double speed = Math.Sqrt(
                                   Math.Pow(currentWorldPosition.X - previousWorldPosition.X, 2)
                                   + Math.Pow(currentWorldPosition.Y - previousWorldPosition.Y, 2) + Math.Pow(
                                       currentWorldPosition.Z - previousWorldPosition.Z,
                                       2)) / duration;

                // if (speed < 200)
                driverInfo.Speed = Velocity.FromMs(speed);
            }

            if (computeNewSpeed)
            {
                _pcarsConnector.PreviousTickInfo[driverInfo.DriverName] = driverInfo;
            }
        }

        public void Reset()
        {
            PitTriggerTimes.Clear();
            DriversInPits.Clear();
        }

        // NEED EXTRACT WHEN SUPPORT FOR OTHER SIMS IS ADDED
        public SimulatorDataSet FromPcarsData(PCarsApiStruct data, TimeSpan lastTickDuration)
        {
            SimulatorDataSet simData = new SimulatorDataSet("PCars");

            // PEDAL INFO
            simData.PedalInfo.ThrottlePedalPosition = data.MThrottle;
            simData.PedalInfo.BrakePedalPosition = data.MBrake;
            simData.PedalInfo.ClutchPedalPosition = data.MClutch;

            FillSessionInfo(data, simData, lastTickDuration);
            AddDriversData(data, simData);

            FillPlayersGear(data, simData);

            // WaterSystemInfo
            simData.PlayerInfo.CarInfo.WaterSystemInfo.WaterTemperature =
                Temperature.FromCelsius(data.MWaterTempCelsius);

            // OilSystemInfo
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilPressure = Pressure.FromKiloPascals(data.MOilPressureKPa);
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(data.MOilTempCelsius);

            // Brakes Info            
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature =
                Temperature.FromCelsius(data.MBrakeTempCelsius[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.BrakeTemperature =
                Temperature.FromCelsius(data.MBrakeTempCelsius[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.BrakeTemperature =
                Temperature.FromCelsius(data.MBrakeTempCelsius[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.BrakeTemperature =
                Temperature.FromCelsius(data.MBrakeTempCelsius[3]);

            // Tyre Pressure Info
            /*simData.PlayerCarInfo.WheelsInfo.FrontLeft.TyrePressure = Pressure.FromKiloPascals(data.ty TirePressure.FrontLeft);
            simData.PlayerCarInfo.WheelsInfo.FrontRight.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.FrontRight);
            simData.PlayerCarInfo.WheelsInfo.RearLeft.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.RearLeft);
            simData.PlayerCarInfo.WheelsInfo.RearRight.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.RearRight);*/

            // Front Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.LeftTyreTemp = Temperature.FromCelsius(data.MTyreTemp[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RightTyreTemp = Temperature.FromCelsius(data.MTyreTemp[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.CenterTyreTemp = Temperature.FromCelsius(data.MTyreTemp[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreWear = data.MTyreWear[0];

            // Front Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.LeftTyreTemp = Temperature.FromCelsius(data.MTyreTemp[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RightTyreTemp = Temperature.FromCelsius(data.MTyreTemp[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.CenterTyreTemp =
                Temperature.FromCelsius(data.MTyreTemp[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreWear = data.MTyreWear[1];

            // Rear Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.LeftTyreTemp = Temperature.FromCelsius(data.MTyreTemp[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RightTyreTemp = Temperature.FromCelsius(data.MTyreTemp[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.CenterTyreTemp = Temperature.FromCelsius(data.MTyreTemp[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreWear = data.MTyreWear[2];

            // Rear Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.LeftTyreTemp = Temperature.FromCelsius(data.MTyreTemp[3]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RightTyreTemp = Temperature.FromCelsius(data.MTyreTemp[3]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.CenterTyreTemp = Temperature.FromCelsius(data.MTyreTemp[3]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreWear = data.MTyreWear[3];

            // Fuel
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(data.MFuelCapacity);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining =
                Volume.FromLiters(data.MFuelCapacity * data.MFuelLevel);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelPressure = Pressure.FromKiloPascals(data.MFuelPressureKPa);

            // Acceleration
            simData.PlayerInfo.CarInfo.Acceleration.XinMs = data.MLocalAcceleration[0];
            simData.PlayerInfo.CarInfo.Acceleration.YinMs = data.MLocalAcceleration[1];
            simData.PlayerInfo.CarInfo.Acceleration.ZinMs = data.MLocalAcceleration[2];

            return simData;
        }

        private static void FillPlayersGear(PCarsApiStruct data, SimulatorDataSet simData)
        {
            switch (data.MGear)
            {
                case 0:
                    simData.PlayerInfo.CarInfo.CurrentGear = "N";
                    break;
                case -1:
                    simData.PlayerInfo.CarInfo.CurrentGear = "R";
                    break;
                case -2:
                    simData.PlayerInfo.CarInfo.CurrentGear = string.Empty;
                    break;
                default:
                    simData.PlayerInfo.CarInfo.CurrentGear = data.MGear.ToString();
                    break;
            }
        }

        private void FillSessionInfo(PCarsApiStruct pCarsData, SimulatorDataSet simData, TimeSpan lastTickDuration)
        {
            if (pCarsData.MGameState == 2)
            {
                _pcarsConnector.SessionTime = _pcarsConnector.SessionTime.Add(lastTickDuration);
            }

            if (pCarsData.MSessionState == (int)ESessionState.SessionInvalid
                || (pCarsData.MSessionState == (int)ESessionState.SessionRace && pCarsData.MRaceState == 1))
            {
                _pcarsConnector.SessionTime = TimeSpan.Zero;
            }

            simData.SessionInfo.SessionTime = _pcarsConnector.SessionTime;

            simData.SessionInfo.WeatherInfo.AirTemperature = Temperature.FromCelsius(pCarsData.MAmbientTemperature);
            simData.SessionInfo.WeatherInfo.TrackTemperature = Temperature.FromCelsius(pCarsData.MTrackTemperature);
            simData.SessionInfo.WeatherInfo.RainIntensity = (int)(pCarsData.MRainDensity * 100);
            simData.SessionInfo.TrackInfo.LayoutLength = pCarsData.MTrackLength;
            simData.SessionInfo.IsActive = true; // (eRaceState)pcarsData.mRaceState == eRaceState.RACESTATE_RACING 

            // || (eRaceState)pcarsData.mRaceState == eRaceState.RACESTATE_FINISHED;
            switch ((ESessionState)pCarsData.MSessionState)
            {
                case ESessionState.SessionPractice:
                case ESessionState.SessionTest:
                case ESessionState.SessionTimeAttack:
                    simData.SessionInfo.SessionType = SessionType.Practice;
                    break;
                case ESessionState.SessionQualify:
                    simData.SessionInfo.SessionType = SessionType.Qualification;
                    break;
                case ESessionState.SessionRace:
                    simData.SessionInfo.SessionType = SessionType.Race;
                    break;
                case ESessionState.SessionInvalid:
                    simData.SessionInfo.SessionType = SessionType.Na;
                    break;
            }

            switch ((ERaceState)pCarsData.MRaceState)
            {
                case ERaceState.RacestateNotStarted:
                    simData.SessionInfo.SessionPhase = SessionPhase.Countdown;
                    break;
                case ERaceState.RacestateRacing:
                    simData.SessionInfo.SessionPhase = SessionPhase.Green;
                    break;
                case ERaceState.RacestateRetired:
                case ERaceState.RacestateDnf:
                case ERaceState.RacestateDisqualified:
                case ERaceState.RacestateFinished:
                    simData.SessionInfo.SessionPhase = SessionPhase.Checkered;
                    break;
                case ERaceState.RacestateInvalid:
                    break;
                case ERaceState.RacestateMax:
                    break;
                default:
                    simData.SessionInfo.SessionPhase = SessionPhase.Green;
                    break;

            }
            if (simData.SessionInfo.SessionPhase == SessionPhase.Countdown
                && simData.SessionInfo.SessionType != SessionType.Race)
            {
                simData.SessionInfo.SessionPhase = SessionPhase.Green;
            }

            simData.SessionInfo.TrackInfo.TrackName = pCarsData.MTrackLocation;
            simData.SessionInfo.TrackInfo.TrackLayoutName = pCarsData.MTrackVariation;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (pCarsData.MEventTimeRemaining != -1)
            {
                simData.SessionInfo.SessionLengthType = SessionLengthType.Time;
                simData.SessionInfo.SessionTimeRemaining = pCarsData.MEventTimeRemaining;
            }
            else if (pCarsData.MLapsInEvent != 0)
            {
                simData.SessionInfo.SessionLengthType = SessionLengthType.Laps;
                simData.SessionInfo.TotalNumberOfLaps = (int)pCarsData.MLapsInEvent;
            }
        }

        private bool ShouldCheckPits(SimulatorDataSet dataSet)
        {
            if (dataSet.SessionInfo.SessionType != SessionType.Race)
            {
                return dataSet.SessionInfo.IsActive;
            }

            return dataSet.SessionInfo.SessionTime.TotalMilliseconds > _pitRaceTimeCheckDelay;
        }

        private void AddPitsInfo(SimulatorDataSet dataSet)
        {
            TrackDetails trackDetails =
                TrackDetails.GetTrackDetails(dataSet.SessionInfo.TrackInfo.TrackName, dataSet.SessionInfo.TrackInfo.TrackLayoutName);
            if (trackDetails == null)
            {
                AddPitsUsingTrackDistance(dataSet);
                return;
            }

            foreach (var driverInfo in dataSet.DriversInfo)
            {
                driverInfo.InPits = false;
                var driverName = driverInfo.DriverName;
                if (_driversInPits.Contains(driverName))
                {
                    if (trackDetails.AtPitExit(driverInfo) || (driverInfo.LapDistance > 300 && dataSet.SessionInfo.TrackInfo.LayoutLength - driverInfo.LapDistance > 700))
                    {
                        _driversInPits.Remove(driverName);
                        continue;
                    }

                    driverInfo.InPits = true;
                    continue;
                }

                if (!trackDetails.AtPitEntry(driverInfo) && !AddPitsUsingTrackDistance(dataSet, driverInfo, true)
                    && (dataSet.SessionInfo.SessionType == SessionType.Race
                        || (dataSet.SessionInfo.SessionTime.TotalSeconds > 5)))
                {
                    continue;
                }

                driverInfo.InPits = true;
                _driversInPits.Add(driverInfo.DriverName);
            }
        }


        private void AddPitsUsingTrackDistance(SimulatorDataSet dataSet)
        {
            if (!ShouldCheckPits(dataSet))
            {
                return;
            }

            foreach (var driverInfo in dataSet.DriversInfo)
            {
                var inPits = AddPitsUsingTrackDistance(dataSet, driverInfo, false);
                driverInfo.InPits = inPits;
                if (inPits && !_driversInPits.Contains(driverInfo.DriverName))
                {
                    _driversInPits.Add(driverInfo.DriverName);
                }
                if (!inPits && _driversInPits.Contains(driverInfo.DriverName))
                {
                    _driversInPits.Remove(driverInfo.DriverName);
                }
            }
        }

        private bool AddPitsUsingTrackDistance(SimulatorDataSet dataSet, DriverInfo driverInfo, bool includeSpeedCheck)
        {
            bool isSpeedZero = !includeSpeedCheck || driverInfo.Speed.InMs < 0.01;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if ((driverInfo.LapDistance != 0 || isSpeedZero) && _pitTriggerTimes.ContainsKey(driverInfo.DriverName))
            {
                _pitTriggerTimes.Remove(driverInfo.DriverName);
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (driverInfo.LapDistance != 0)
            {
                if (_driversInPits.Contains(driverInfo.DriverName))
                {
                    _driversInPits.Remove(driverInfo.DriverName);
                }
                return false;
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (driverInfo.LapDistance == 0 && isSpeedZero)
            {
                if (!_pitTriggerTimes.ContainsKey(driverInfo.DriverName))
                {
                    _pitTriggerTimes[driverInfo.DriverName] = dataSet.SessionInfo.SessionTime.Add(_pitTimeDelay);
                    return false;
                }

                return dataSet.SessionInfo.SessionTime > _pitTriggerTimes[driverInfo.DriverName];
            }

            return false;
        }
    }
}