using System;
using System.Linq;
using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.DataModel.Snapshot;
using SecondMonitor.DataModel.Snapshot.Drivers;
using SecondMonitor.PluginManager.Extensions;
using SecondMonitor.RF2Connector.SharedMemory.rFactor2Data;

namespace SecondMonitor.RF2Connector.SharedMemory
{
    internal class RF2DataConvertor
    {

        private DriverInfo _lastPlayer = new DriverInfo();
        private int _lastPlayerId = -1;
        private const int maxConsecutivePackagesIgnored = 200;

        private int currentlyIgnoredPackage = 0;

        public SimulatorDataSet CreateSimulatorDataSet(Rf2FullData rfData)
        {
            SimulatorDataSet simData = new SimulatorDataSet("RFactor");
            simData.SimulatorSourceInfo.HasLapTimeInformation = true;
            simData.SimulatorSourceInfo.SimNotReportingEndOfOutLapCorrectly = true;
            simData.SimulatorSourceInfo.SectorTimingSupport = DataInputSupport.FULL;

            FillSessionInfo(rfData, simData);
            AddDriversData(simData, rfData);

            if (_lastPlayerId == -1)
            {
                return simData;

            }

            rF2VehicleTelemetry playerF2VehicleTelemetry = rfData.telemetry.mVehicles.First(x => x.mID == _lastPlayerId);

            FillPlayersGear(rfData, simData);

            // PEDAL INFO
            AddPedalInfo(rfData, simData);

            // WaterSystemInfo
            AddWaterSystemInfo(rfData, simData);

            // OilSystemInfo
            AddOilSystemInfo(rfData, simData);

            // Brakes Info
            AddBrakesInfo(rfData, simData);

            // Tyre Pressure Info
            AddTyresAndFuelInfo(simData, playerF2VehicleTelemetry);

            // Acceleration
            AddAcceleration(simData, playerF2VehicleTelemetry);

            currentlyIgnoredPackage = 0;
            return simData;
        }

        private void AddAcceleration(SimulatorDataSet simData, rF2VehicleTelemetry playerVehicleTelemetry)
        {
            simData.PlayerInfo.CarInfo.Acceleration.XinMs = playerVehicleTelemetry.mLocalAccel.x;
            simData.PlayerInfo.CarInfo.Acceleration.YinMs = playerVehicleTelemetry.mLocalAccel.y;
            simData.PlayerInfo.CarInfo.Acceleration.ZinMs = playerVehicleTelemetry.mLocalAccel.z;
        }

        private void AddTyresAndFuelInfo(SimulatorDataSet simData, rF2VehicleTelemetry playerVehicleTelemetry)
        {
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyrePressure =
                Pressure.FromKiloPascals(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontLeft].mPressure);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyrePressure =
                Pressure.FromKiloPascals(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontRight].mPressure);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyrePressure =
                Pressure.FromKiloPascals(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearLeft].mPressure);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyrePressure =
                Pressure.FromKiloPascals(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearRight].mPressure);



            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreWear =
                1 - playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontLeft].mWear;
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreWear =
                1 - playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontRight].mWear;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreWear =
                1 - playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearLeft].mWear;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreWear =
                1 - playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearRight].mWear;

            // Front Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.LeftTyreTemp =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontLeft].mTemperature[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RightTyreTemp =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontLeft].mTemperature[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.CenterTyreTemp =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontLeft].mTemperature[1]);


            // Front Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.LeftTyreTemp =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontRight].mTemperature[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RightTyreTemp =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontRight].mTemperature[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.CenterTyreTemp =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontRight].mTemperature[1]);


            // Rear Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.LeftTyreTemp =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearLeft].mTemperature[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RightTyreTemp =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearLeft].mTemperature[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.CenterTyreTemp =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearLeft].mTemperature[1]);

            // Rear Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.LeftTyreTemp =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearRight].mTemperature[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RightTyreTemp =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearRight].mTemperature[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.CenterTyreTemp =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearRight].mTemperature[1]);

            // Fuel System
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(playerVehicleTelemetry.mFuelCapacity);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining = Volume.FromLiters(playerVehicleTelemetry.mFuel);
        }

        private static void AddBrakesInfo(Rf2FullData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature = Temperature.FromKelvin(data.Wheel[(int)RfWheelIndex.FrontLeft].BrakeTemp);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.BrakeTemperature = Temperature.FromKelvin(data.Wheel[(int)RfWheelIndex.FrontRight].BrakeTemp);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.BrakeTemperature = Temperature.FromKelvin(data.Wheel[(int)RfWheelIndex.RearLeft].BrakeTemp);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.BrakeTemperature = Temperature.FromKelvin(data.Wheel[(int)RfWheelIndex.RearRight].BrakeTemp);
        }

        private static void AddOilSystemInfo(Rf2FullData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(data.EngineOilTemp);
        }

        private static void AddWaterSystemInfo(Rf2FullData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WaterSystemInfo.WaterTemperature = Temperature.FromCelsius(data.EngineWaterTemp);
        }

        private static void AddPedalInfo(Rf2FullData data, SimulatorDataSet simData)
        {
            simData.PedalInfo.ThrottlePedalPosition = data.UnfilteredThrottle;
            simData.PedalInfo.BrakePedalPosition = data.UnfilteredBrake;
            simData.PedalInfo.ClutchPedalPosition = data.UnfilteredClutch;
        }


        private static void FillPlayersGear(Rf2FullData data, SimulatorDataSet simData)
        {
            switch (data.Gear)
            {
                case 0:
                    simData.PlayerInfo.CarInfo.CurrentGear = "N";
                    break;
                case -1:
                    simData.PlayerInfo.CarInfo.CurrentGear = "R";
                    break;
                case -2:
                    simData.PlayerInfo.CarInfo.CurrentGear = String.Empty;
                    break;
                default:
                    simData.PlayerInfo.CarInfo.CurrentGear = data.Gear.ToString();
                    break;
            }
        }

        internal void AddDriversData(SimulatorDataSet data, Rf2FullData rfData)
        {
            if (rfData.NumVehicles < 1)
            {
                return;
            }

            data.DriversInfo = new DriverInfo[rfData.NumVehicles];
            DriverInfo playersInfo = null;

            for (int i = 0; i < rfData.NumVehicles; i++)
            {
                RfVehicleInfo rfVehicleInfo = rfData.Vehicle[i];
                DriverInfo driverInfo = CreateDriverInfo(rfData, rfVehicleInfo);

                if (driverInfo.IsPlayer)
                {
                    playersInfo = driverInfo;
                    driverInfo.CurrentLapValid = true;
                }
                else
                {
                    driverInfo.CurrentLapValid = true;
                }

                data.DriversInfo[i] = driverInfo;
                if (driverInfo.Position == 1)
                {
                    data.SessionInfo.LeaderCurrentLap = driverInfo.CompletedLaps + 1;
                    data.LeaderInfo = driverInfo;
                }

                AddLappingInformation(data, rfData, driverInfo);
                FillTimingInfo(driverInfo, rfVehicleInfo, rfData);

                if (driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Finished && !driverInfo.IsPlayer && driverInfo.Position > _lastPlayer.Position)
                {
                    driverInfo.CompletedLaps--;
                    driverInfo.FinishStatus = DriverInfo.DriverFinishStatus.None;
                }
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

            Distance distance = Point3D.GetDistance(driver.WorldPosition, _lastPlayer.WorldPosition);
            if (distance.DistanceInM > 200)
            {
                currentlyIgnoredPackage++;
                if (currentlyIgnoredPackage < maxConsecutivePackagesIgnored)
                {
                    throw new RF2InvalidPackageException("Players distance was :" + distance.DistanceInM);
                }
            }

        }

        internal void ValidateLapBasedOnSurface(DriverInfo driverInfo, RfVehicleInfo rfVehicleInfo)
        {
            if (!driverInfo.CurrentLapValid)
            {
                return;
            }
        }

        internal void FillTimingInfo(DriverInfo driverInfo, RfVehicleInfo rfVehicleInfo, Rf2FullData Rf2FullData)
        {
            driverInfo.Timing.LastSector1Time = CreateTimeSpan(rfVehicleInfo.CurSector1);
            driverInfo.Timing.LastSector2Time = CreateTimeSpan(rfVehicleInfo.CurSector2 - rfVehicleInfo.CurSector1);
            driverInfo.Timing.LastSector3Time = CreateTimeSpan(rfVehicleInfo.LastLapTime - rfVehicleInfo.LastSector2);
            driverInfo.Timing.LastLapTime = CreateTimeSpan(rfVehicleInfo.LastLapTime);
            driverInfo.Timing.CurrentSector = rfVehicleInfo.Sector == 0 ? 3 : rfVehicleInfo.Sector;
        }

        private TimeSpan CreateTimeSpan(double seconds)
        {
            return seconds > 0 ? TimeSpan.FromSeconds(seconds) : TimeSpan.Zero;
        }

        private void AddLappingInformation(SimulatorDataSet data, Rf2FullData rfData, DriverInfo driverInfo)
        {
            if (data.SessionInfo.SessionType == SessionType.Race && _lastPlayer != null
                && _lastPlayer.CompletedLaps != 0)
            {
                driverInfo.IsBeingLappedByPlayer =
                    driverInfo.TotalDistance < (_lastPlayer.TotalDistance - rfData.LapDist  * 0.5);
                driverInfo.IsLappingPlayer =
                    _lastPlayer.TotalDistance < (driverInfo.TotalDistance - rfData.LapDist * 0.5);
            }
        }

        private DriverInfo CreateDriverInfo(Rf2FullData rfData, RfVehicleInfo rfVehicleInfo)
        {
            DriverInfo driverInfo = new DriverInfo
                                        {
                                            DriverName = StringExtensions.FromArray(rfVehicleInfo.DriverName),
                                            CompletedLaps = rfVehicleInfo.TotalLaps,
                                            CarName = StringExtensions.FromArray(rfVehicleInfo.VehicleClass),
                                            InPits = rfVehicleInfo.InPits == 1
                                        };

            driverInfo.IsPlayer = rfVehicleInfo.IsPlayer == 1;
            driverInfo.Position = rfVehicleInfo.Place;
            driverInfo.Speed = Velocity.FromMs(rfVehicleInfo.Speed);
            driverInfo.LapDistance = rfVehicleInfo.LapDist;
            driverInfo.TotalDistance = rfVehicleInfo.TotalLaps * rfData.LapDist + rfVehicleInfo.LapDist;
            driverInfo.FinishStatus = FromRFStatus(rfVehicleInfo.FinishStatus);
            driverInfo.WorldPosition = new Point3D(Distance.FromMeters(rfVehicleInfo.Pos.X), Distance.FromMeters(rfVehicleInfo.Pos.Y), Distance.FromMeters(rfVehicleInfo.Pos.Z));
            ComputeDistanceToPlayer(_lastPlayer, driverInfo, rfData);
            return driverInfo;
        }

        internal static void ComputeDistanceToPlayer(DriverInfo player, DriverInfo driverInfo, Rf2FullData Rf2FullData)
        {
            if (player == null)
            {
                return;
            }

            if (driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Dq || driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Dnf ||
                driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Dnq || driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Dns)
            {
                driverInfo.DistanceToPlayer = Double.MaxValue;
                return;
            }

            double trackLength = Rf2FullData.LapDist;
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

        internal void FillSessionInfo(Rf2FullData data, SimulatorDataSet simData)
        {
            // Timing
            simData.SessionInfo.SessionTime = TimeSpan.FromSeconds(data.CurrentET);
            simData.SessionInfo.TrackInfo.LayoutLength = data.LapDist;
            simData.SessionInfo.TrackInfo.TrackName = StringExtensions.FromArray(data.TrackName);
            simData.SessionInfo.TrackInfo.TrackLayoutName = String.Empty;
            simData.SessionInfo.WeatherInfo.AirTemperature = Temperature.FromCelsius(data.AmbientTemp);
            simData.SessionInfo.WeatherInfo.TrackTemperature = Temperature.FromCelsius(data.TrackTemp);

            if (data.TrackTemp == 0 && data.Session == 0 && data.GamePhase == 0
                && String.IsNullOrEmpty(simData.SessionInfo.TrackInfo.TrackName)
                && String.IsNullOrEmpty(StringExtensions.FromArray(data.VehicleName)) && data.LapDist == 0)
            {
                simData.SessionInfo.SessionType = SessionType.Na;
                simData.SessionInfo.SessionPhase = SessionPhase.Countdown;
                return;
            }

            switch ((RfSessionType)data.Session)
            {
                case RfSessionType.NA:
                    simData.SessionInfo.SessionType = SessionType.Na;
                    break;
                case RfSessionType.Practice1:
                case RfSessionType.Practice2:
                case RfSessionType.Practice3:
                case RfSessionType.TestDay:
                    simData.SessionInfo.SessionType = SessionType.Practice;
                    break;
                case RfSessionType.Qualification:
                    simData.SessionInfo.SessionType = SessionType.Qualification;
                    break;
                case RfSessionType.WarmUp:
                    simData.SessionInfo.SessionType = SessionType.WarmUp;
                    break;
                case RfSessionType.Race1:
                case RfSessionType.Race2:
                case RfSessionType.Race3:
                case RfSessionType.Race4:
                case RfSessionType.Race5:
                    simData.SessionInfo.SessionType = SessionType.Race;
                    break;
                default:
                    simData.SessionInfo.SessionType = SessionType.Practice;
                    break;
            }

            switch ((RfGamePhase)data.GamePhase)
            {
                case RfGamePhase.Garage:
                    break;
                case RfGamePhase.WarmUp:
                case RfGamePhase.GridWalk:
                case RfGamePhase.Formation:
                case RfGamePhase.Countdown:
                    simData.SessionInfo.SessionPhase = SessionPhase.Countdown;
                    break;
                case RfGamePhase.SessionStopped:
                case RfGamePhase.GreenFlag:
                case RfGamePhase.FullCourseYellow:
                    simData.SessionInfo.SessionPhase = SessionPhase.Green;
                    break;
                case RfGamePhase.SessionOver:
                    simData.SessionInfo.SessionPhase = SessionPhase.Checkered;
                    break;
            }

            simData.SessionInfo.IsActive = simData.SessionInfo.SessionType != SessionType.Na;



            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (data.EndET > 0)
            {
                simData.SessionInfo.SessionLengthType = SessionLengthType.Time;
                simData.SessionInfo.SessionTimeRemaining =
                    data.EndET - data.CurrentET > 0 ? data.EndET - data.CurrentET : 0;
            }
            else
            {
                simData.SessionInfo.SessionLengthType = SessionLengthType.Laps;
                simData.SessionInfo.TotalNumberOfLaps = data.MaxLaps;
            }
        }

        internal static DriverInfo.DriverFinishStatus FromRFStatus(int finishStatus)
        {
            switch ((RfFinishStatus)finishStatus)
            {
                case RfFinishStatus.None:
                    return DriverInfo.DriverFinishStatus.Na;
                case RfFinishStatus.Dnf:
                    return DriverInfo.DriverFinishStatus.Dnf;
                case RfFinishStatus.Dq:
                    return DriverInfo.DriverFinishStatus.Dq;
                case RfFinishStatus.Finished:
                    return DriverInfo.DriverFinishStatus.Finished;
                default:
                    return DriverInfo.DriverFinishStatus.Na;
            }
        }
    }
}