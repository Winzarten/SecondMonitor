using System;
using System.Collections.Generic;
using SecondMonitor.DataModel;
using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.DataModel.Drivers;
using SecondMonitor.PCarsConnector.enums;

namespace SecondMonitor.PCarsConnector
{
    public class PCarsConvertor
    {
        private readonly PCarsConnector _pCarsConnector;
        private SimulatorDataSet _lastSpeedComputationSet;
        private DriverInfo _lastPlayer = new DriverInfo();
        private readonly TimeSpan _pitTimeDelay = TimeSpan.FromMilliseconds(2000);
        private readonly int _pitRaceTimeCheckDelay = 20000;
        private readonly Dictionary<string, TimeSpan> _pitTriggerTimes;
        private readonly HashSet<string> _driversInPits;

        public PCarsConvertor(PCarsConnector pCarsConnector)
        {
            _pCarsConnector = pCarsConnector;
            _pitTriggerTimes = new Dictionary<string, TimeSpan>();
            _driversInPits = new HashSet<string>();
        }

        public Dictionary<string, TimeSpan> PitTriggerTimes => _pitTriggerTimes;
        public HashSet<string> DriversInPits => _driversInPits;

        private void AddDriversData(PCarsApiStruct pCarsData, SimulatorDataSet data, TimeSpan lastTickDuration)
        {
            if (pCarsData.MNumParticipants == -1)
                return;
            TrackDetails trackDetails =
                TrackDetails.GetTrackDetails(data.SessionInfo.TrackName, data.SessionInfo.TrackLayoutName);
            data.DriversInfo = new DataModel.Drivers.DriverInfo[pCarsData.MNumParticipants];
            DriverInfo playersInfo = null;
            var computeSpeed = data.SessionInfo.SessionTime.TotalSeconds > _pCarsConnector.NextSpeedComputation;
            for (int i = 0; i < pCarsData.MNumParticipants; i++)
            {
                PCarsApiParticipantStruct pcarsDriverData = pCarsData.MParticipantData[i];
                DataModel.Drivers.DriverInfo driverInfo =
                    new DataModel.Drivers.DriverInfo
                    {
                        DriverName = pcarsDriverData.MName,
                        CompletedLaps = (int) pcarsDriverData.MLapsCompleted,
                        CarName = pCarsData.MCarClassName,
                        InPits = false,
                        IsPlayer = i == pCarsData.MViewedParticipantIndex,
                        Position = (int) pcarsDriverData.MRacePosition,
                        LapDistance = pcarsDriverData.MCurrentLapDistance,
                        FinishStatus = DriverInfo.DriverFinishStatus.None,
                        CurrentLapValid = true,                        
                        WorldPostion = new Point3D(pcarsDriverData.MWorldPosition[0], pcarsDriverData.MWorldPosition[1],
                            pcarsDriverData.MWorldPosition[2])
                        
            };
                driverInfo.TotalDistance = driverInfo.CompletedLaps * data.SessionInfo.LayoutLength + driverInfo.LapDistance;
                //System.Text.Encoding.UTF8.GetString(r3rDriverData.DriverInfo.).Replace("\0", "");
                // r3rDriverData.InPitlane == 1;                                                
                if (driverInfo.IsPlayer)
                {
                    playersInfo = driverInfo;
                    driverInfo.CurrentLapValid = !pCarsData.MLapInvalidated;
                }
                else
                    driverInfo.CurrentLapValid = true;
                data.DriversInfo[i] = driverInfo;
                if (String.IsNullOrEmpty(driverInfo.DriverName))
                    throw new PCarsConnector.NameNotFilledException("Name not filled for driver with index " + i);
                AddSpeedInfo(data, computeSpeed, driverInfo);
                if (data.SessionInfo.SessionType == SessionInfo.SessionTypeEnum.Race && _lastPlayer != null && _lastPlayer.CompletedLaps != 0)
                {
                    driverInfo.IsBeingLappedByPlayer = driverInfo.TotalDistance < (_lastPlayer.TotalDistance - data.SessionInfo.LayoutLength * 0.5);
                    driverInfo.IsLapingPlayer = _lastPlayer.TotalDistance < (driverInfo.TotalDistance - data.SessionInfo.LayoutLength * 0.5);
                }
                if (driverInfo.Position == 1)
                {
                    data.LeaderInfo = driverInfo;
                    data.SessionInfo.LeaderCurrentLap = driverInfo.CompletedLaps + 1;
                }
            }
            if (computeSpeed)
                _pCarsConnector.NextSpeedComputation += 0.5;
            if (playersInfo != null)
            {
                ComputeDistanceToPlayer(playersInfo, data);
                data.PlayerInfo = playersInfo;
                _lastPlayer = playersInfo;
            }
            AddPitsInfo(data);
            if(computeSpeed)
                _lastSpeedComputationSet = data;

        }

        private void AddSpeedInfo(SimulatorDataSet data, bool computeNewSpeed, DriverInfo driverInfo)
        {
            if (!computeNewSpeed && _pCarsConnector.PreviousTickInfo.ContainsKey(driverInfo.DriverName))
            {
                driverInfo.Speed = _pCarsConnector.PreviousTickInfo[driverInfo.DriverName].Speed;
            }
            if (_pCarsConnector.PreviousTickInfo.ContainsKey(driverInfo.DriverName) && computeNewSpeed && _lastSpeedComputationSet != null)  
            {
                Point3D currentWorldPosition = driverInfo.WorldPostion;
                Point3D previousWorldPosition =
                    _pCarsConnector.PreviousTickInfo[driverInfo.DriverName].WorldPostion;
                double duration = data.SessionInfo.SessionTime
                    .Subtract(_lastSpeedComputationSet.SessionInfo.SessionTime).TotalSeconds;
                //double speed = lastTickDuration.TotalMilliseconds;
                double speed = Math.Sqrt(Math.Pow(currentWorldPosition.X - previousWorldPosition.X, 2) +
                                         Math.Pow(currentWorldPosition.Y - previousWorldPosition.Y, 2) +
                                         Math.Pow(currentWorldPosition.Z - previousWorldPosition.Z, 2)) / duration;
                //if (speed < 200)
                    driverInfo.Speed = Velocity.FromMs(speed);
            }
            if (computeNewSpeed)
            {                
                _pCarsConnector.PreviousTickInfo[driverInfo.DriverName] = driverInfo;
            }
        }

        public void Reset()
        {
            PitTriggerTimes.Clear();
            DriversInPits.Clear();
        }

        private static void ComputeDistanceToPlayer(DataModel.Drivers.DriverInfo player, SimulatorDataSet data)
        {
            Single trackLength = data.SessionInfo.LayoutLength;
            Single playerLapDistance = player.LapDistance;
            foreach (var driverInfo in data.DriversInfo)
            {
                Single distanceToPlayer = playerLapDistance - driverInfo.LapDistance;
                if (distanceToPlayer < -(trackLength / 2))
                    distanceToPlayer = distanceToPlayer + trackLength;
                if (distanceToPlayer > (trackLength / 2))
                    distanceToPlayer = distanceToPlayer - trackLength;
                driverInfo.DistanceToPlayer = distanceToPlayer;
            }
        }

        //NEED EXTRACT WHEN SUPPORT FOR OTHER SIMS IS ADDED
        public SimulatorDataSet FromPcarsData(PCarsApiStruct data, TimeSpan lastTickDuration)
        {
            SimulatorDataSet simData = new SimulatorDataSet("PCars");
            //PEDAL INFO
            simData.PedalInfo.ThrottlePedalPosition = data.MThrottle;
            simData.PedalInfo.BrakePedalPosition = data.MBrake;
            simData.PedalInfo.ClutchPedalPosition = data.MClutch;

            FillSessionInfo(data, simData, lastTickDuration);
            AddDriversData(data, simData, lastTickDuration);

            //WaterSystemInfo
            simData.PlayerInfo.CarInfo.WaterSystmeInfo.WaterTemperature =
                Temperature.FromCelsius(data.MWaterTempCelsius);

            //OilSystemInfo
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilPressure = Pressure.FromKiloPascals(data.MOilPressureKPa);
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(data.MOilTempCelsius);

            //Brakes Info            
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature =
                Temperature.FromCelsius(data.MBrakeTempCelsius[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.BrakeTemperature =
                Temperature.FromCelsius(data.MBrakeTempCelsius[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.BrakeTemperature =
                Temperature.FromCelsius(data.MBrakeTempCelsius[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.BrakeTemperature =
                Temperature.FromCelsius(data.MBrakeTempCelsius[3]);

            //Tyre Pressure Info
            /*simData.PlayerCarInfo.WheelsInfo.FrontLeft.TyrePressure = Pressure.FromKiloPascals(data.ty TirePressure.FrontLeft);
            simData.PlayerCarInfo.WheelsInfo.FrontRight.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.FrontRight);
            simData.PlayerCarInfo.WheelsInfo.RearLeft.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.RearLeft);
            simData.PlayerCarInfo.WheelsInfo.RearRight.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.RearRight);*/

            //Front Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.LeftTyreTemp = Temperature.FromCelsius(data.MTyreTemp[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RightTyreTemp = Temperature.FromCelsius(data.MTyreTemp[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.CenterTyreTemp = Temperature.FromCelsius(data.MTyreTemp[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreWear = data.MTyreWear[0];

            //Front Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.LeftTyreTemp = Temperature.FromCelsius(data.MTyreTemp[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RightTyreTemp = Temperature.FromCelsius(data.MTyreTemp[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.CenterTyreTemp =
                Temperature.FromCelsius(data.MTyreTemp[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreWear = data.MTyreWear[1];

            //Rear Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.LeftTyreTemp = Temperature.FromCelsius(data.MTyreTemp[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RightTyreTemp = Temperature.FromCelsius(data.MTyreTemp[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.CenterTyreTemp = Temperature.FromCelsius(data.MTyreTemp[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreWear = data.MTyreWear[2];

            //Rear Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.LeftTyreTemp = Temperature.FromCelsius(data.MTyreTemp[3]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RightTyreTemp = Temperature.FromCelsius(data.MTyreTemp[3]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.CenterTyreTemp = Temperature.FromCelsius(data.MTyreTemp[3]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreWear = data.MTyreWear[3];

            //Fuel
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(data.MFuelCapacity);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining =
                Volume.FromLiters(data.MFuelCapacity * data.MFuelLevel);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelPressure = Pressure.FromKiloPascals(data.MFuelPressureKPa);

            //Acceleration
            simData.PlayerInfo.CarInfo.Acceleration.XinMs = data.MLocalAcceleration[0];
            simData.PlayerInfo.CarInfo.Acceleration.YinMs = data.MLocalAcceleration[1];
            simData.PlayerInfo.CarInfo.Acceleration.ZinMs = data.MLocalAcceleration[2];


            return simData;
        }

        private void FillSessionInfo(PCarsApiStruct pCarsData, SimulatorDataSet simData, TimeSpan lastTickDuration)
        {

            if (pCarsData.MGameState == 2)
            {
                _pCarsConnector.SessionTime = _pCarsConnector.SessionTime.Add(lastTickDuration);
            }
            if (pCarsData.MSessionState == (int) ESessionState.SessionInvalid ||
                (pCarsData.MSessionState == (int) ESessionState.SessionRace && pCarsData.MRaceState == 1))
                _pCarsConnector.SessionTime = new TimeSpan(0);
            simData.SessionInfo.SessionTime = _pCarsConnector.SessionTime;


            simData.SessionInfo.WeatherInfo.AirTemperature = Temperature.FromCelsius(pCarsData.MAmbientTemperature);
            simData.SessionInfo.WeatherInfo.TrackTemperature = Temperature.FromCelsius(pCarsData.MTrackTemperature);
            simData.SessionInfo.WeatherInfo.RainIntensity = (int) (pCarsData.MRainDensity * 100);
            simData.SessionInfo.LayoutLength = pCarsData.MTrackLength;
            simData.SessionInfo.IsActive = true; // (eRaceState)pCarsData.mRaceState == eRaceState.RACESTATE_RACING 
            //|| (eRaceState)pCarsData.mRaceState == eRaceState.RACESTATE_FINISHED;
            switch ((ESessionState) pCarsData.MSessionState)
            {
                case ESessionState.SessionPractice:
                case ESessionState.SessionTest:
                case ESessionState.SessionTimeAttack:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.Practice;
                    break;
                case ESessionState.SessionQualify:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.Qualification;
                    break;
                case ESessionState.SessionRace:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.Race;
                    break;
                case ESessionState.SessionInvalid:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.Na;
                    break;

            }
            switch ((ERaceState) pCarsData.MRaceState)
            {
                case ERaceState.RacestateNotStarted:
                    simData.SessionInfo.SessionPhase = SessionInfo.SessionPhaseEnum.Countdown;
                    break;
                case ERaceState.RacestateRacing:
                    simData.SessionInfo.SessionPhase = SessionInfo.SessionPhaseEnum.Green;
                    break;
                case ERaceState.RacestateRetired:
                case ERaceState.RacestateDnf:
                case ERaceState.RacestateDisqualified:
                case ERaceState.RacestateFinished:
                    simData.SessionInfo.SessionPhase = SessionInfo.SessionPhaseEnum.Checkered;
                    break;

            }
            if (simData.SessionInfo.SessionPhase == SessionInfo.SessionPhaseEnum.Countdown &&
                simData.SessionInfo.SessionType != SessionInfo.SessionTypeEnum.Race)
                simData.SessionInfo.SessionPhase = SessionInfo.SessionPhaseEnum.Green;
            simData.SessionInfo.TrackName = pCarsData.MTrackLocation;
            simData.SessionInfo.TrackLayoutName = pCarsData.MTrackVariation;

            if (pCarsData.MEventTimeRemaining != -1)
            {
                simData.SessionInfo.SessionLengthType = SessionInfo.SessionLengthTypeEnum.Time;
                simData.SessionInfo.SessionTimeRemaining = pCarsData.MEventTimeRemaining;
            }
            else if (pCarsData.MLapsInEvent != 0)
            {
                simData.SessionInfo.SessionLengthType = SessionInfo.SessionLengthTypeEnum.Laps;
                simData.SessionInfo.TotalNumberOfLaps = (int) pCarsData.MLapsInEvent;
            }

        }

        private bool ShouldCheckPits(SimulatorDataSet dataSet)
        {
            if (dataSet.SessionInfo.SessionType != SessionInfo.SessionTypeEnum.Race)
                return dataSet.SessionInfo.IsActive;
            return dataSet.SessionInfo.SessionTime.TotalMilliseconds > _pitRaceTimeCheckDelay;
        }

        private void AddPitsInfo(SimulatorDataSet dataSet)
        {
            TrackDetails trackDetails =
                TrackDetails.GetTrackDetails(dataSet.SessionInfo.TrackName, dataSet.SessionInfo.TrackLayoutName);
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
                    if (trackDetails.AtPitExit(driverInfo) || (driverInfo.LapDistance > 300 && dataSet.SessionInfo.LayoutLength - driverInfo.LapDistance > 500))
                    {
                        _driversInPits.Remove(driverName);
                        continue;
                    }
                    driverInfo.InPits = true;
                    continue;
                }
                if (!trackDetails.AtPitEntry(driverInfo) && !AddPitsUsingTrackDistance(dataSet,driverInfo, true) &&
                    (dataSet.SessionInfo.SessionType == SessionInfo.SessionTypeEnum.Race ||
                     (dataSet.SessionInfo.SessionTime.TotalSeconds > 5))) continue;
                driverInfo.InPits = true;
                _driversInPits.Add(driverInfo.DriverName);
            }
        }
        

        private void AddPitsUsingTrackDistance(SimulatorDataSet dataSet)
        {
            if (!ShouldCheckPits(dataSet))
                return;

            foreach (var driverInfo in dataSet.DriversInfo)
            {
                var inPits = AddPitsUsingTrackDistance(dataSet, driverInfo, false);
                driverInfo.InPits = inPits;
                if (inPits && !_driversInPits.Contains(driverInfo.DriverName))
                    _driversInPits.Add(driverInfo.DriverName);
                if (!inPits && _driversInPits.Contains(driverInfo.DriverName))
                    _driversInPits.Remove(driverInfo.DriverName);
             }
        }

        private bool AddPitsUsingTrackDistance(SimulatorDataSet dataSet, DriverInfo driverInfo, bool includeSpeedCheck)
        {
            bool isSpeedZero = !includeSpeedCheck || driverInfo.Speed.InMs < 0.01;
            if ((driverInfo.LapDistance != 0 || isSpeedZero) && _pitTriggerTimes.ContainsKey(driverInfo.DriverName))
                _pitTriggerTimes.Remove(driverInfo.DriverName);
            if (driverInfo.LapDistance != 0)
            {
                if (_driversInPits.Contains(driverInfo.DriverName))
                    _driversInPits.Remove(driverInfo.DriverName);
                return false;
            }
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