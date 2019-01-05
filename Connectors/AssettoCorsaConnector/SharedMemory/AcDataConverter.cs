namespace SecondMonitor.AssettoCorsaConnector.SharedMemory
{
    using System;
    using System.Collections.Generic;

    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;
    using PluginManager.Extensions;

    public class AcDataConverter
    {
        private const double MaxWear = 91;

        private readonly AssettoCorsaConnector _connector;
        private readonly Dictionary<string, TimeSpan[]> _currentSectorTimes;

        private readonly AssettoCorsaStartObserver _startObserver;

        private DriverInfo _lastPlayer;
        private int? _sectorLength;

        public AcDataConverter(AssettoCorsaConnector assettoCorsaConnector)
        {
            _connector = assettoCorsaConnector;
            _currentSectorTimes = new Dictionary<string, TimeSpan[]>();
            _sectorLength = null;
            _startObserver = new AssettoCorsaStartObserver();
        }

        public SimulatorDataSet CreateSimulatorDataSet(AssettoCorsaShared acData)
        {
            SimulatorDataSet simData = new SimulatorDataSet("Assetto Corsa");
            simData.SimulatorSourceInfo.HasLapTimeInformation = true;
            simData.SimulatorSourceInfo.OutLapIsValid = true;
            simData.SimulatorSourceInfo.SimNotReportingEndOfOutLapCorrectly = true;
            simData.SimulatorSourceInfo.ForceLapOverTime = true;
            simData.SimulatorSourceInfo.GlobalTyreCompounds = true;
            simData.SimulatorSourceInfo.SectorTimingSupport = DataInputSupport.SpOnly;

            FillSessionInfo(acData, simData);
            AddDriversData(simData, acData);

            FillPlayerCarInfo(acData, simData);

            // PEDAL INFO
            AddPedalInfo(acData, simData);

            // WaterSystemInfo
            AddWaterSystemInfo(simData);

            // OilSystemInfo
            AddOilSystemInfo(simData);

            // Brakes Info
            AddBrakesInfo(acData, simData);

            // Tyre Pressure Info
            AddTyresAndFuelInfo(simData, acData);

            // Acceleration
            AddAcceleration(simData, acData);


            _startObserver.Observe(simData);
            return simData;
        }

        private void FillPlayerCarInfo(AssettoCorsaShared acData, SimulatorDataSet simData)
        {
            FillPlayersGear(acData, simData);
            simData.PlayerInfo.CarInfo.EngineRpm = acData.AcsPhysics.rpms;
        }

        private static void AddAcceleration(SimulatorDataSet simData, AssettoCorsaShared acData)
        {
            simData.PlayerInfo.CarInfo.Acceleration.XinMs = acData.AcsPhysics.accG[0] * 9.8;
            simData.PlayerInfo.CarInfo.Acceleration.YinMs = acData.AcsPhysics.accG[1] * 9.8;
            simData.PlayerInfo.CarInfo.Acceleration.ZinMs = acData.AcsPhysics.accG[2] * 9.8;
        }

        private static void AddTyresAndFuelInfo(SimulatorDataSet simData, AssettoCorsaShared acData)
        {
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyrePressure.ActualQuantity = Pressure.FromPsi(acData.AcsPhysics.wheelsPressure[(int)AcWheels.FL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyrePressure.ActualQuantity = Pressure.FromPsi(acData.AcsPhysics.wheelsPressure[(int)AcWheels.FR]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyrePressure.ActualQuantity = Pressure.FromPsi(acData.AcsPhysics.wheelsPressure[(int)AcWheels.RL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyrePressure.ActualQuantity = Pressure.FromPsi(acData.AcsPhysics.wheelsPressure[(int)AcWheels.RR]);

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreWear.ActualWear = GetACTyreWear(acData.AcsPhysics.tyreWear[(int)AcWheels.FL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreWear.ActualWear = GetACTyreWear(acData.AcsPhysics.tyreWear[(int)AcWheels.FR]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreWear.ActualWear = GetACTyreWear(acData.AcsPhysics.tyreWear[(int)AcWheels.RL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreWear.ActualWear = GetACTyreWear(acData.AcsPhysics.tyreWear[(int)AcWheels.FR]);

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.Rps = acData.AcsPhysics.wheelAngularSpeed[(int)AcWheels.FL];
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.Rps = acData.AcsPhysics.wheelAngularSpeed[(int)AcWheels.FR];
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.Rps = acData.AcsPhysics.wheelAngularSpeed[(int)AcWheels.RL];
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.Rps = acData.AcsPhysics.wheelAngularSpeed[(int)AcWheels.FR];



            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RideHeight =  Distance.FromMeters(acData.AcsPhysics.rideHeight[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RideHeight = Distance.FromMeters(acData.AcsPhysics.rideHeight[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RideHeight = Distance.FromMeters(acData.AcsPhysics.rideHeight[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RideHeight = Distance.FromMeters(acData.AcsPhysics.rideHeight[1]);

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.SuspensionTravel = Distance.FromMeters(acData.AcsPhysics.suspensionTravel[(int) AcWheels.FL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.SuspensionTravel = Distance.FromMeters(acData.AcsPhysics.suspensionTravel[(int)AcWheels.FR]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.SuspensionTravel = Distance.FromMeters(acData.AcsPhysics.suspensionTravel[(int)AcWheels.RL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.SuspensionTravel = Distance.FromMeters(acData.AcsPhysics.suspensionTravel[(int)AcWheels.FR]);

            // Front Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreTempI[(int)AcWheels.FL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreTempM[(int)AcWheels.FL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreTempO[(int)AcWheels.FL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreCoreTemperature.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreCoreTemperature[(int)AcWheels.FL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreType = acData.AcsGraphic.tyreCompound;

            // Front Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreTempI[(int)AcWheels.FR]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreTempM[(int)AcWheels.FR]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreTempO[(int)AcWheels.FR]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreCoreTemperature.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreCoreTemperature[(int)AcWheels.FR]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreType = acData.AcsGraphic.tyreCompound;


            // Rear Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreTempI[(int)AcWheels.RL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreTempM[(int)AcWheels.RL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreTempO[(int)AcWheels.RL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreCoreTemperature.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreCoreTemperature[(int)AcWheels.RL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreType = acData.AcsGraphic.tyreCompound;

            // Rear Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreTempI[(int)AcWheels.RR]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreTempM[(int)AcWheels.RR]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreTempO[(int)AcWheels.RR]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreCoreTemperature.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.tyreCoreTemperature[(int)AcWheels.RR]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreType = acData.AcsGraphic.tyreCompound;

            // Fuel System
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(acData.AcsStatic.maxFuel);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining = Volume.FromLiters(acData.AcsPhysics.fuel);
        }

        private static double GetACTyreWear(double acReportedTyreWear)
        {
            double percentages =(acReportedTyreWear - MaxWear) / (100.0 - MaxWear);
            return 1 - percentages;
        }

        public void ResetConverter()
        {
            _currentSectorTimes.Clear();
            _sectorLength = null;
        }

        private static void AddBrakesInfo(AssettoCorsaShared acData, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.brakeTemp[(int)AcWheels.FL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.brakeTemp[(int)AcWheels.FR]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.brakeTemp[(int)AcWheels.RL]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(acData.AcsPhysics.brakeTemp[(int)AcWheels.RR]);
        }

        private static void AddOilSystemInfo(SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(100);
        }

        private static void AddWaterSystemInfo(SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WaterSystemInfo.WaterTemperature = Temperature.FromCelsius(100);
        }

        private static void AddPedalInfo(AssettoCorsaShared acData, SimulatorDataSet simData)
        {
            simData.InputInfo.ThrottlePedalPosition = acData.AcsPhysics.gas;
            simData.InputInfo.BrakePedalPosition = acData.AcsPhysics.brake;
            simData.InputInfo.ClutchPedalPosition = 1 - acData.AcsPhysics.clutch;
            simData.InputInfo.SteeringInput = acData.AcsPhysics.steerAngle;
        }


        private static void FillPlayersGear(AssettoCorsaShared acData, SimulatorDataSet simData)
        {
            int gear = acData.AcsPhysics.gear - 1;
            switch (gear)
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
                    simData.PlayerInfo.CarInfo.CurrentGear = gear.ToString();
                    break;
            }
        }

        internal void AddDriversData(SimulatorDataSet data, AssettoCorsaShared acData)
        {
            if (acData.AcsSecondMonitor.numVehicles  < 1)
            {
                return;
            }

            data.DriversInfo = new DriverInfo[acData.AcsSecondMonitor.numVehicles];
            DriverInfo playersInfo = null;

            for (int i = 0; i < acData.AcsSecondMonitor.numVehicles; i++)
            {
                AcsVehicleInfo acVehicleInfo = acData.AcsSecondMonitor.vehicle[i];
                DriverInfo driverInfo = CreateDriverInfo(acData, acVehicleInfo, data);

                driverInfo.CurrentLapValid = acVehicleInfo.currentLapInvalid == 0;
                if (driverInfo.IsPlayer)
                {
                    playersInfo = driverInfo;
                    driverInfo.CurrentLapValid &= acData.AcsPhysics.numberOfTyresOut != 4;
                }


                data.DriversInfo[i] = driverInfo;
                if (driverInfo.Position == 1)
                {
                    data.SessionInfo.LeaderCurrentLap = driverInfo.CompletedLaps + 1;
                    data.LeaderInfo = driverInfo;
                }

                AddLappingInformation(data, acData, driverInfo);
                FillTimingInfo(driverInfo, acVehicleInfo);


            }
            CheckValidityByPlayer(playersInfo);
            _lastPlayer = playersInfo;
            if (playersInfo != null)
            {
                data.PlayerInfo = playersInfo;
            }
        }

        private void CheckValidityByPlayer(DriverInfo driver)
        {
            if (_lastPlayer == null || driver == null || (!_lastPlayer.InPits && driver.InPits))
            {
                return;
            }

            /*Distance distance = Point3D.GetDistance(driver.WorldPosition, _lastPlayer.WorldPosition);
            if (distance.InMeters > 200)
            {
                currentlyIgnoredPackage++;
                if (currentlyIgnoredPackage < MaxConsecutivePackagesIgnored)
                {
                    throw new RF2InvalidPackageException("Players distance was :" + distance.InMeters);
                }
            }*/

        }

        internal void FillTimingInfo(DriverInfo driverInfo, AcsVehicleInfo acVehicleInfo)
        {
            driverInfo.Timing.LastLapTime = CreateTimeSpan(acVehicleInfo.lastLapTimeMS);
            driverInfo.Timing.CurrentLapTime = CreateTimeSpan(acVehicleInfo.currentLapTimeMS);
            driverInfo.Timing.CurrentSector = -1;

            if (_sectorLength == null)
            {
                return;
            }

            int currentSector = (int)driverInfo.LapDistance / _sectorLength.Value;
            TimeSpan[] splits = GetCurrentSplitTimes(driverInfo.DriverName);
            if (splits == null || splits.Length <= currentSector)
            {
                return;
            }

            splits[currentSector] = driverInfo.Timing.CurrentLapTime;
            driverInfo.Timing.CurrentSector = currentSector + 1;
            driverInfo.Timing.LastSector1Time = splits[0];
            driverInfo.Timing.LastSector2Time = splits[1] - splits[0];
            driverInfo.Timing.LastSector3Time = splits[2] - splits[1];
            driverInfo.Timing.CurrentSectorTime = splits[currentSector];

        }

        private TimeSpan[] GetCurrentSplitTimes(string playerName)
        {
            return GetSplitTimes(playerName, _currentSectorTimes);
        }

        private TimeSpan[] GetSplitTimes(string playerName, Dictionary<string, TimeSpan[]> splitsDictionary)
        {
            if (!splitsDictionary.ContainsKey(playerName))
            {
                splitsDictionary[playerName] = new[] { TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero };
            }

            return splitsDictionary[playerName];
        }

        private TimeSpan CreateTimeSpan(double miliseconds)
        {
            return miliseconds > 0 ? TimeSpan.FromMilliseconds(miliseconds) : TimeSpan.Zero;
        }

        private void AddLappingInformation(SimulatorDataSet data, AssettoCorsaShared acData, DriverInfo driverInfo)
        {
            if (data.SessionInfo.SessionType == SessionType.Race && _lastPlayer != null
                && _lastPlayer.CompletedLaps != 0)
            {
                driverInfo.IsBeingLappedByPlayer =
                    driverInfo.TotalDistance < (_lastPlayer.TotalDistance - acData.AcsStatic.trackSPlineLength * 0.5);
                driverInfo.IsLappingPlayer =
                    _lastPlayer.TotalDistance < (driverInfo.TotalDistance - acData.AcsStatic.trackSPlineLength * 0.5);
            }
        }

        private DriverInfo CreateDriverInfo(AssettoCorsaShared acData, AcsVehicleInfo acVehicleInfo, SimulatorDataSet dataSet)
        {
            DriverInfo driverInfo = new DriverInfo
            {
                DriverName = StringExtensions.FromArray(acVehicleInfo.driverName),
                CompletedLaps = acVehicleInfo.lapCount,
                CarName = FormatACName(StringExtensions.FromArray(acVehicleInfo.carModel)),
                CarClassName = "N/A"
            };

            driverInfo.InPits = acVehicleInfo.isCarInPit == 1 || acVehicleInfo.isCarInPitlane == 1;


            driverInfo.IsPlayer = acVehicleInfo.carId == 0;
            driverInfo.Position = dataSet.SessionInfo.SessionType == SessionType.Race ? acVehicleInfo.carRealTimeLeaderboardPosition + 1 : acVehicleInfo.carLeaderboardPosition;
            driverInfo.Speed = Velocity.FromMs(acVehicleInfo.speedMS);
            driverInfo.LapDistance = acData.AcsStatic.trackSPlineLength * acVehicleInfo.spLineLength;
            driverInfo.TotalDistance = acVehicleInfo.lapCount * acData.AcsStatic.trackSPlineLength + acVehicleInfo.spLineLength * acData.AcsStatic.trackSPlineLength;
            driverInfo.FinishStatus = FromAcStatus(acVehicleInfo.finishedStatus);
            driverInfo.WorldPosition = new Point3D(Distance.FromMeters(acVehicleInfo.worldPosition.x), Distance.FromMeters(acVehicleInfo.worldPosition.y), Distance.FromMeters(acVehicleInfo.worldPosition.z));
            ComputeDistanceToPlayer(_lastPlayer, driverInfo, acData);
            return driverInfo;
        }

        internal static void ComputeDistanceToPlayer(DriverInfo player, DriverInfo driverInfo, AssettoCorsaShared assettoCorsaShared)
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

            double trackLength = assettoCorsaShared.AcsStatic.trackSPlineLength;
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

        internal void FillSessionInfo(AssettoCorsaShared data, SimulatorDataSet simData)
        {
            // Timing
            simData.SessionInfo.SessionTime = _connector.SessionTime;
            simData.SessionInfo.TrackInfo.LayoutLength = Distance.FromMeters(data.AcsStatic.trackSPlineLength);
            simData.SessionInfo.TrackInfo.TrackName = FormatACName(data.AcsStatic.track);
            simData.SessionInfo.TrackInfo.TrackLayoutName = data.AcsStatic.trackConfiguration;
            simData.SessionInfo.WeatherInfo.AirTemperature = Temperature.FromCelsius(data.AcsPhysics.airTemp);
            simData.SessionInfo.WeatherInfo.TrackTemperature = Temperature.FromCelsius(data.AcsPhysics.roadTemp);
            simData.SessionInfo.WeatherInfo.RainIntensity = 0;

            switch (data.AcsGraphic.session)
            {
                case AcSessionType.AC_DRAG:
                case AcSessionType.AC_PRACTICE:
                case AcSessionType.AC_TIME_ATTACK:
                case AcSessionType.AC_HOT_LAP:
                    simData.SessionInfo.SessionType = SessionType.Practice;
                    break;
                case AcSessionType.AC_QUALIFY:
                    simData.SessionInfo.SessionType = SessionType.Qualification;
                    break;
                case AcSessionType.AC_DRIFT:
                case AcSessionType.AC_RACE:
                    simData.SessionInfo.SessionType = SessionType.Race;
                    break;
                default:
                    simData.SessionInfo.SessionType = SessionType.Na;
                    break;
            }

            simData.SessionInfo.SessionPhase = SessionPhase.Green;

            simData.SessionInfo.IsActive = data.AcsGraphic.status == AcStatus.AC_LIVE;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if ((data.AcsStatic.isTimedRace == 1 || simData.SessionInfo.SessionType != SessionType.Race)
                && data.AcsGraphic.sessionTimeLeft < 0)
            {

                simData.SessionInfo.SessionLengthType = SessionLengthType.Time;
                simData.SessionInfo.SessionTimeRemaining = 120000;
            }
            else if (data.AcsStatic.isTimedRace == 1 || simData.SessionInfo.SessionType != SessionType.Race)
            {
                simData.SessionInfo.SessionLengthType = SessionLengthType.Time;
                simData.SessionInfo.SessionTimeRemaining = data.AcsGraphic.sessionTimeLeft / 1000;
            }
            else
            {
                simData.SessionInfo.SessionLengthType = SessionLengthType.Laps;
                simData.SessionInfo.TotalNumberOfLaps = data.AcsGraphic.numberOfLaps;
            }

            _sectorLength = (int)simData.SessionInfo.TrackInfo.LayoutLength.InMeters / 3;

        }

        internal static DriverFinishStatus FromAcStatus(int finishStatus)
        {
            return finishStatus == 0 ? DriverFinishStatus.None : DriverFinishStatus.Finished;
        }

        private static string FormatACName(string name)
        {
            return name.Replace("ks_", string.Empty).Replace("_", " ");
        }
    }
}