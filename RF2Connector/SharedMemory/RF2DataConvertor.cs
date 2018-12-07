namespace SecondMonitor.RF2Connector.SharedMemory
{
    using System;
    using System.Linq;

    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;
    using PluginManager.Extensions;
    using rFactor2Data;

    internal class RF2DataConvertor
    {

        private const int MaxConsecutivePackagesIgnored = 200;
        private DriverInfo _lastPlayer = new DriverInfo();
        private int _lastPlayerId = -1;

        private int currentlyIgnoredPackage = 0;

        public SimulatorDataSet CreateSimulatorDataSet(Rf2FullData rfData)
        {
            SimulatorDataSet simData = new SimulatorDataSet("RFactor 2");
            simData.SimulatorSourceInfo.HasLapTimeInformation = true;
            simData.SimulatorSourceInfo.SimNotReportingEndOfOutLapCorrectly = true;
            simData.SimulatorSourceInfo.InvalidateLapBySector = true;
            simData.SimulatorSourceInfo.SectorTimingSupport = DataInputSupport.Full;

            FillSessionInfo(rfData, simData);
            AddDriversData(simData, rfData);

            if (_lastPlayerId == -1)
            {
                return simData;

            }


            rF2VehicleTelemetry playerF2VehicleTelemetry =
                rfData.telemetry.mVehicles.First(x => x.mID == _lastPlayerId);

            FillPlayersGear(playerF2VehicleTelemetry, simData);

            // PEDAL INFO
            AddPedalInfo(playerF2VehicleTelemetry, simData);

            // WaterSystemInfo
            AddWaterSystemInfo(playerF2VehicleTelemetry, simData);

            // OilSystemInfo
            AddOilSystemInfo(playerF2VehicleTelemetry, simData);

            // Brakes Info
            AddBrakesInfo(playerF2VehicleTelemetry, simData);

            // Tyre Pressure Info
            AddTyresAndFuelInfo(simData, playerF2VehicleTelemetry);

            // Acceleration
            AddAcceleration(simData, playerF2VehicleTelemetry);

            AddFlags(rfData, simData);

            currentlyIgnoredPackage = 0;


            return simData;
        }

        private void AddFlags(Rf2FullData rfData, SimulatorDataSet simData)
        {
            if ((rFactor2Constants.rF2GamePhase)rfData.scoring.mScoringInfo.mGamePhase == rFactor2Constants.rF2GamePhase.FullCourseYellow)
            {
                simData.SessionInfo.ActiveFlags.Add(FlagKind.FullCourseYellow);
                return;
            }

            if (rfData.scoring.mScoringInfo.mSectorFlag[0] == 1)
            {
                simData.SessionInfo.ActiveFlags.Add(FlagKind.YellowSector1);
            }

            if (rfData.scoring.mScoringInfo.mSectorFlag[1] == 1)
            {
                simData.SessionInfo.ActiveFlags.Add(FlagKind.YellowSector2);
            }

            if (rfData.scoring.mScoringInfo.mSectorFlag[2] == 1)
            {
                simData.SessionInfo.ActiveFlags.Add(FlagKind.YellowSector3);
            }
        }

        private void AddAcceleration(SimulatorDataSet simData, rF2VehicleTelemetry playerVehicleTelemetry)
        {
            simData.PlayerInfo.CarInfo.Acceleration.XinMs = playerVehicleTelemetry.mLocalAccel.x;
            simData.PlayerInfo.CarInfo.Acceleration.YinMs = playerVehicleTelemetry.mLocalAccel.y;
            simData.PlayerInfo.CarInfo.Acceleration.ZinMs = playerVehicleTelemetry.mLocalAccel.z;
        }

        private void AddTyresAndFuelInfo(SimulatorDataSet simData, rF2VehicleTelemetry playerVehicleTelemetry)
        {
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyrePressure.ActualQuantity =
                Pressure.FromKiloPascals(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontLeft].mPressure);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyrePressure.ActualQuantity =
                Pressure.FromKiloPascals(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontRight].mPressure);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyrePressure.ActualQuantity =
                Pressure.FromKiloPascals(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearLeft].mPressure);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyrePressure.ActualQuantity =
                Pressure.FromKiloPascals(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearRight].mPressure);



            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreWear.ActualWear =
                1 - playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontLeft].mWear;
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreWear.ActualWear =
                1 - playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontRight].mWear;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreWear.ActualWear =
                1 - playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearLeft].mWear;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreWear.ActualWear =
                1 - playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearRight].mWear;

            // Front Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.LeftTyreTemp.ActualQuantity =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontLeft].mTemperature[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RightTyreTemp.ActualQuantity =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontLeft].mTemperature[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.CenterTyreTemp.ActualQuantity =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontLeft].mTemperature[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreCoreTemperature.ActualQuantity =
                Temperature.FromKelvin(Math.Min(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontLeft].mTireCarcassTemperature, 2000));


            // Front Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.LeftTyreTemp.ActualQuantity =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontRight].mTemperature[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RightTyreTemp.ActualQuantity =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontRight].mTemperature[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.CenterTyreTemp.ActualQuantity =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontRight].mTemperature[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreCoreTemperature.ActualQuantity =
                Temperature.FromKelvin(Math.Min(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.FrontRight].mTireCarcassTemperature, 2000));


            // Rear Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.LeftTyreTemp.ActualQuantity =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearLeft].mTemperature[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RightTyreTemp.ActualQuantity =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearLeft].mTemperature[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.CenterTyreTemp.ActualQuantity =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearLeft].mTemperature[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreCoreTemperature.ActualQuantity =
                Temperature.FromKelvin(Math.Min(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearLeft].mTireCarcassTemperature, 2000));

            // Rear Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.LeftTyreTemp.ActualQuantity =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearRight].mTemperature[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RightTyreTemp.ActualQuantity =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearRight].mTemperature[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.CenterTyreTemp.ActualQuantity =
                Temperature.FromKelvin(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearRight].mTemperature[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreCoreTemperature.ActualQuantity =
                Temperature.FromKelvin(Math.Min(playerVehicleTelemetry.mWheels[(int)rFactor2Constants.rF2WheelIndex.RearRight].mTireCarcassTemperature, 2000));

            // Fuel System
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(playerVehicleTelemetry.mFuelCapacity);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining = Volume.FromLiters(playerVehicleTelemetry.mFuel);
        }

        private static void AddBrakesInfo(rF2VehicleTelemetry playerVehicleTelemetry, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature.ActualQuantity = Temperature.FromKelvin(
                playerVehicleTelemetry.mWheels[(int) rFactor2Constants.rF2WheelIndex.FrontLeft].mBrakeTemp);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.BrakeTemperature.ActualQuantity = Temperature.FromKelvin(
                playerVehicleTelemetry.mWheels[(int) rFactor2Constants.rF2WheelIndex.FrontRight].mBrakeTemp);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.BrakeTemperature.ActualQuantity = Temperature.FromKelvin(
                playerVehicleTelemetry.mWheels[(int) rFactor2Constants.rF2WheelIndex.RearLeft].mBrakeTemp);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.BrakeTemperature.ActualQuantity = Temperature.FromKelvin(
                playerVehicleTelemetry.mWheels[(int) rFactor2Constants.rF2WheelIndex.RearRight].mBrakeTemp);
        }

        private static void AddOilSystemInfo(rF2VehicleTelemetry playerVehicleTelemetry, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(playerVehicleTelemetry.mEngineOilTemp);
        }

        private static void AddWaterSystemInfo(rF2VehicleTelemetry playerVehicleTelemetry, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WaterSystemInfo.WaterTemperature = Temperature.FromCelsius(playerVehicleTelemetry.mEngineWaterTemp);
        }

        private static void AddPedalInfo(rF2VehicleTelemetry playerVehicleTelemetry, SimulatorDataSet simData)
        {
            simData.InputInfo.ThrottlePedalPosition = playerVehicleTelemetry.mUnfilteredThrottle;
            simData.InputInfo.BrakePedalPosition = playerVehicleTelemetry.mUnfilteredBrake;
            simData.InputInfo.ClutchPedalPosition = playerVehicleTelemetry.mUnfilteredClutch;
            simData.InputInfo.SteeringInput = playerVehicleTelemetry.mUnfilteredSteering;
        }


        private static void FillPlayersGear(rF2VehicleTelemetry playerVehicleTelemetry, SimulatorDataSet simData)
        {
            switch (playerVehicleTelemetry.mGear)
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
                    simData.PlayerInfo.CarInfo.CurrentGear = playerVehicleTelemetry.mGear.ToString();
                    break;
            }
        }

        internal void AddDriversData(SimulatorDataSet data, Rf2FullData rfData)
        {
            if (rfData.scoring.mScoringInfo.mNumVehicles < 1)
            {
                return;
            }

            data.DriversInfo = new DriverInfo[rfData.scoring.mScoringInfo.mNumVehicles];
            DriverInfo playersInfo = null;

            for (int i = 0; i < rfData.scoring.mScoringInfo.mNumVehicles; i++)
            {
                rF2VehicleScoring rF2VehicleScoring = rfData.scoring.mVehicles[i];
                DriverInfo driverInfo = CreateDriverInfo(rfData, rF2VehicleScoring);

                if (driverInfo.IsPlayer)
                {
                    playersInfo = driverInfo;
                    driverInfo.CurrentLapValid = true;
                    _lastPlayerId = rF2VehicleScoring.mID;
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
                FillTimingInfo(driverInfo, rF2VehicleScoring, rfData);

                if (driverInfo.FinishStatus == DriverFinishStatus.Finished && !driverInfo.IsPlayer && driverInfo.Position > _lastPlayer.Position)
                {
                    driverInfo.CompletedLaps--;
                    driverInfo.FinishStatus = DriverFinishStatus.None;
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
            if (distance.InMeters > 200)
            {
                currentlyIgnoredPackage++;
                if (currentlyIgnoredPackage < MaxConsecutivePackagesIgnored)
                {
                    throw new RF2InvalidPackageException("Players distance was :" + distance.InMeters);
                }
            }

        }

        internal void FillTimingInfo(DriverInfo driverInfo, rF2VehicleScoring rfVehicleInfo, Rf2FullData Rf2FullData)
        {
            driverInfo.Timing.LastSector1Time = CreateTimeSpan(rfVehicleInfo.mCurSector1);
            driverInfo.Timing.LastSector2Time = CreateTimeSpan(rfVehicleInfo.mCurSector2 - rfVehicleInfo.mCurSector1);
            driverInfo.Timing.LastSector3Time = CreateTimeSpan(rfVehicleInfo.mLastLapTime - rfVehicleInfo.mLastSector2);
            driverInfo.Timing.LastLapTime = CreateTimeSpan(rfVehicleInfo.mLastLapTime);
            driverInfo.Timing.CurrentSector = rfVehicleInfo.mSector == 0 ? 3 : rfVehicleInfo.mSector;

            switch (driverInfo.Timing.CurrentSector)
            {
                case 1:
                    driverInfo.Timing.CurrentLapTime = CreateTimeSpan(rfVehicleInfo.mCurSector1);
                    break;
                case 2:
                    driverInfo.Timing.CurrentLapTime = CreateTimeSpan(rfVehicleInfo.mCurSector2);
                    break;
                case 0:
                    driverInfo.Timing.CurrentLapTime = CreateTimeSpan(rfVehicleInfo.mLastLapTime);
                    break;
            }
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
                    driverInfo.TotalDistance < (_lastPlayer.TotalDistance - rfData.scoring.mScoringInfo.mLapDist  * 0.5);
                driverInfo.IsLappingPlayer =
                    _lastPlayer.TotalDistance < (driverInfo.TotalDistance - rfData.scoring.mScoringInfo.mLapDist * 0.5);
            }
        }

        private DriverInfo CreateDriverInfo(Rf2FullData rfData, rF2VehicleScoring rfVehicleInfo)
        {
            DriverInfo driverInfo = new DriverInfo
                                        {
                                            DriverName = StringExtensions.FromArray(rfVehicleInfo.mDriverName),
                                            CompletedLaps = rfVehicleInfo.mTotalLaps,
                                            CarName = StringExtensions.FromArray(rfVehicleInfo.mVehicleName),
                                            InPits = rfVehicleInfo.mInPits == 1
                                        };

            driverInfo.IsPlayer = rfVehicleInfo.mIsPlayer == 1;
            driverInfo.Position = rfVehicleInfo.mPlace;
            driverInfo.Speed = Velocity.FromMs(Math.Sqrt((rfVehicleInfo.mLocalVel.x * rfVehicleInfo.mLocalVel.x)
                                                         + (rfVehicleInfo.mLocalVel.y * rfVehicleInfo.mLocalVel.y)
                                                         + (rfVehicleInfo.mLocalVel.z * rfVehicleInfo.mLocalVel.z)));
            driverInfo.LapDistance = rfVehicleInfo.mLapDist;
            driverInfo.TotalDistance = rfVehicleInfo.mTotalLaps * rfData.scoring.mScoringInfo.mLapDist + rfVehicleInfo.mLapDist;
            driverInfo.FinishStatus = FromRFStatus(rfVehicleInfo.mFinishStatus);
            driverInfo.WorldPosition = new Point3D(Distance.FromMeters(-rfVehicleInfo.mPos.x), Distance.FromMeters(rfVehicleInfo.mPos.y), Distance.FromMeters(rfVehicleInfo.mPos.z));
            ComputeDistanceToPlayer(_lastPlayer, driverInfo, rfData);
            return driverInfo;
        }

        internal static void ComputeDistanceToPlayer(DriverInfo player, DriverInfo driverInfo, Rf2FullData rf2FullData)
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

            double trackLength = rf2FullData.scoring.mScoringInfo.mLapDist;
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
            simData.SessionInfo.SessionTime = TimeSpan.FromSeconds(data.scoring.mScoringInfo.mCurrentET);
            simData.SessionInfo.TrackInfo.LayoutLength = Distance.FromMeters(data.scoring.mScoringInfo.mLapDist);
            simData.SessionInfo.TrackInfo.TrackName = StringExtensions.FromArray(data.scoring.mScoringInfo.mTrackName);
            simData.SessionInfo.TrackInfo.TrackLayoutName = string.Empty;
            simData.SessionInfo.WeatherInfo.AirTemperature = Temperature.FromCelsius(data.scoring.mScoringInfo.mAmbientTemp);
            simData.SessionInfo.WeatherInfo.TrackTemperature = Temperature.FromCelsius(data.scoring.mScoringInfo.mTrackTemp);
            simData.SessionInfo.WeatherInfo.RainIntensity = (int)(data.scoring.mScoringInfo.mRaining * 100);

            if (data.scoring.mScoringInfo.mTrackTemp == 0 && data.scoring.mScoringInfo.mSession == 0 && data.scoring.mScoringInfo.mGamePhase == 0
                && string.IsNullOrEmpty(simData.SessionInfo.TrackInfo.TrackName)
                && data.scoring.mScoringInfo.mLapDist == 0)
            {
                simData.SessionInfo.SessionType = SessionType.Na;
                simData.SessionInfo.SessionPhase = SessionPhase.Countdown;
                return;
            }

            switch (data.scoring.mScoringInfo.mSession)
            {
                case -1:
                    simData.SessionInfo.SessionType = SessionType.Na;
                    break;
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    simData.SessionInfo.SessionType = SessionType.Practice;
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                    simData.SessionInfo.SessionType = SessionType.Qualification;
                    break;
                case 9:
                    simData.SessionInfo.SessionType = SessionType.WarmUp;
                    break;
                case 10:
                case 11:
                case 12:
                case 13:
                    simData.SessionInfo.SessionType = SessionType.Race;
                    break;
                default:
                    simData.SessionInfo.SessionType = SessionType.Practice;
                    break;
            }

            switch ((rFactor2Constants.rF2GamePhase)data.scoring.mScoringInfo.mGamePhase)
            {
                case rFactor2Constants.rF2GamePhase.Garage:
                    break;
                case rFactor2Constants.rF2GamePhase.WarmUp:
                case rFactor2Constants.rF2GamePhase.GridWalk:
                case rFactor2Constants.rF2GamePhase.Formation:
                case rFactor2Constants.rF2GamePhase.Countdown:
                    simData.SessionInfo.SessionPhase = SessionPhase.Countdown;
                    break;
                case rFactor2Constants.rF2GamePhase.SessionStopped:
                case rFactor2Constants.rF2GamePhase.FullCourseYellow:
                case rFactor2Constants.rF2GamePhase.GreenFlag:
                    simData.SessionInfo.SessionPhase = SessionPhase.Green;
                    break;
                case rFactor2Constants.rF2GamePhase.SessionOver:
                    simData.SessionInfo.SessionPhase = SessionPhase.Checkered;
                    break;
            }

            simData.SessionInfo.IsActive = simData.SessionInfo.SessionType != SessionType.Na;



            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (data.scoring.mScoringInfo.mEndET > 0)
            {
                simData.SessionInfo.SessionLengthType = SessionLengthType.Time;
                simData.SessionInfo.SessionTimeRemaining =
                    data.scoring.mScoringInfo.mEndET - data.scoring.mScoringInfo.mCurrentET > 0 ? data.scoring.mScoringInfo.mEndET - data.scoring.mScoringInfo.mCurrentET : 0;
            }
            else
            {
                simData.SessionInfo.SessionLengthType = SessionLengthType.Laps;
                simData.SessionInfo.TotalNumberOfLaps = data.scoring.mScoringInfo.mMaxLaps;
            }
        }

        internal static DriverFinishStatus FromRFStatus(int finishStatus)
        {
            switch ((rFactor2Constants.rF2FinishStatus)finishStatus)
            {
                case rFactor2Constants.rF2FinishStatus.None:
                    return DriverFinishStatus.Na;
                case rFactor2Constants.rF2FinishStatus.Dnf:
                    return DriverFinishStatus.Dnf;
                case rFactor2Constants.rF2FinishStatus.Dq:
                    return DriverFinishStatus.Dq;
                case rFactor2Constants.rF2FinishStatus.Finished:
                    return DriverFinishStatus.Finished;
                default:
                    return DriverFinishStatus.Na;
            }
        }
    }
}