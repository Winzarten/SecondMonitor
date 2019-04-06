namespace SecondMonitor.PCars2Connector.DataConvertor
{
    using System;
    using System.Linq;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;
    using DataModel.Snapshot.Systems;
    using SharedMemory;
    using PluginManager.Extensions;
    using PluginManager.GameConnector;

    public class PCars2DataConvertor : AbstractDataConvertor
    {
        private DriverInfo _lastPlayer = new DriverInfo();

        public SimulatorDataSet CreateSimulatorDataSet(PCars2SharedMemory pcarsData, TimeSpan sessionTime)
        {
            SimulatorDataSet simData = new SimulatorDataSet("PCars 2");
            simData.SimulatorSourceInfo.HasLapTimeInformation = true;
            simData.SimulatorSourceInfo.OutLapIsValid = true;
            simData.SimulatorSourceInfo.InvalidateLapBySector = true;
            simData.SimulatorSourceInfo.SectorTimingSupport = DataInputSupport.Full;
            simData.SimulatorSourceInfo.TelemetryInfo.ContainsSuspensionTravel = true;
            simData.SimulatorSourceInfo.TelemetryInfo.ContainsSuspensionVelocity = true;

            FillSessionInfo(pcarsData, simData, sessionTime);
            AddDriversData(simData, pcarsData);

            FillPlayerCarInfo(pcarsData, simData);

            // PEDAL INFO
            AddPedalInfo(pcarsData, simData);

            // WaterSystemInfo
            AddWaterSystemInfo(pcarsData, simData);

            // OilSystemInfo
            AddOilSystemInfo(pcarsData, simData);

            // Brakes Info
            AddBrakesInfo(pcarsData, simData);

            // Tyre Pressure Info
            AddTyresAndFuelInfo(pcarsData, simData);

            //Add Additional Player Car Info
            AddPlayerCarInfo(pcarsData, simData);

            // Acceleration
            AddAcceleration(pcarsData, simData);

            if (simData.PlayerInfo?.FinishStatus == DriverFinishStatus.Dns && simData.SessionInfo.SessionType == SessionType.Race)
            {
                simData.SessionInfo.SessionPhase = SessionPhase.Countdown;
            }

            PopulateClassPositions(simData);
            AddActiveFlags(pcarsData, simData);

            return simData;
        }

        private void AddPlayerCarInfo(PCars2SharedMemory data, SimulatorDataSet simData)
        {
            CarInfo playerCar = simData.PlayerInfo.CarInfo;

            playerCar.CarDamageInformation.Bodywork.Damage = data.mAeroDamage;
            playerCar.CarDamageInformation.Engine.Damage = data.mEngineDamage;
            playerCar.CarDamageInformation.Suspension.Damage = data.mSuspensionDamage.Max();

            /*playerCar.SpeedLimiterEngaged = (data.mCarFlags & (int)CarFlags.CarSpeedLimiter) == (int)CarFlags.CarSpeedLimiter;*/

            FillBoostData(data, playerCar);
        }

        private void FillBoostData(PCars2SharedMemory data, CarInfo playerCar)
        {
            BoostSystem boostSystem = playerCar.BoostSystem;

            if (data.mBoostAmount <= 0)
            {
                boostSystem.BoostStatus = BoostStatus.UnAvailable;
                return;
            }

            boostSystem.ActivationsRemaining = (int) data.mBoostAmount;
            boostSystem.BoostStatus = data.mBoostActive ? BoostStatus.InUse : BoostStatus.Available;
        }

        public void AddActiveFlags(PCars2SharedMemory pcarsData, SimulatorDataSet simData)
        {
            for (int i = 0; i < pcarsData.mNumParticipants; i++)
            {
                HighestFlagColor highestFlagColor = (HighestFlagColor)pcarsData.mHighestFlagColours[i];
                if (highestFlagColor == HighestFlagColor.FlagColourNone)
                {
                    continue;
                }

                if (highestFlagColor == HighestFlagColor.FlagColourYellow || highestFlagColor == HighestFlagColor.FlagColourDoubleYellow)
                {
                    int sector = pcarsData.mParticipantData[i].mCurrentSector + 1;
                    switch (sector)
                    {
                        case 1:
                            if (!simData.SessionInfo.ActiveFlags.Contains(FlagKind.YellowSector1))
                            {
                                simData.SessionInfo.ActiveFlags.Add(FlagKind.YellowSector1);
                            }
                            break;
                        case 2:
                            if (!simData.SessionInfo.ActiveFlags.Contains(FlagKind.YellowSector2))
                            {
                                simData.SessionInfo.ActiveFlags.Add(FlagKind.YellowSector2);
                            }
                            break;
                        case 3:
                            if (!simData.SessionInfo.ActiveFlags.Contains(FlagKind.YellowSector3))
                            {
                                simData.SessionInfo.ActiveFlags.Add(FlagKind.YellowSector3);
                            }
                            break;
                    }
                }
            }

        }

        private static void AddAcceleration(PCars2SharedMemory data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.Acceleration.XinMs = data.mLocalAcceleration[0];
            simData.PlayerInfo.CarInfo.Acceleration.YinMs = data.mLocalAcceleration[1];
            simData.PlayerInfo.CarInfo.Acceleration.ZinMs = data.mLocalAcceleration[2];
        }

        private static void AddTyresAndFuelInfo(PCars2SharedMemory data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyrePressure.ActualQuantity = Pressure.FromKiloPascals(data.mAirPressure[(int)WheelIndex.TyreFrontLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyrePressure.ActualQuantity = Pressure.FromKiloPascals(data.mAirPressure[(int)WheelIndex.TyreFrontRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyrePressure.ActualQuantity = Pressure.FromKiloPascals(data.mAirPressure[(int)WheelIndex.TyreRearLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyrePressure.ActualQuantity = Pressure.FromKiloPascals(data.mAirPressure[(int)WheelIndex.TyreRearRight]);

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreWear.ActualWear = data.mTyreWear[(int)WheelIndex.TyreFrontLeft];
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreWear.ActualWear = data.mTyreWear[(int)WheelIndex.TyreFrontRight];
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreWear.ActualWear = data.mTyreWear[(int)WheelIndex.TyreRearLeft];
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreWear.ActualWear = data.mTyreWear[(int)WheelIndex.TyreRearRight];

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.Rps = data.mTyreRPS[(int)WheelIndex.TyreFrontLeft] * -6.283185300;
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.Rps = data.mTyreRPS[(int)WheelIndex.TyreFrontRight]* -6.283185300;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.Rps = data.mTyreRPS[(int)WheelIndex.TyreRearLeft] * -6.283185300;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.Rps = data.mTyreRPS[(int)WheelIndex.TyreRearRight]* -6.283185300;

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.SuspensionTravel = Distance.FromMeters(data.mSuspensionTravel[(int)WheelIndex.TyreFrontLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.SuspensionTravel = Distance.FromMeters(data.mSuspensionTravel[(int)WheelIndex.TyreFrontRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.SuspensionTravel = Distance.FromMeters(data.mSuspensionTravel[(int)WheelIndex.TyreRearLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.SuspensionTravel = Distance.FromMeters(data.mSuspensionTravel[(int)WheelIndex.TyreRearRight]);

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.SuspensionVelocity = Velocity.FromMs(data.mSuspensionVelocity[(int)WheelIndex.TyreFrontLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.SuspensionVelocity = Velocity.FromMs(data.mSuspensionVelocity[(int)WheelIndex.TyreFrontRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.SuspensionVelocity = Velocity.FromMs(data.mSuspensionVelocity[(int)WheelIndex.TyreRearLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.SuspensionVelocity = Velocity.FromMs(data.mSuspensionVelocity[(int)WheelIndex.TyreRearRight]);

            int direDeflatedFlag = (int) TyreFlags.TyreAttached | (int) TyreFlags.TyreInflated;

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.Detached = (data.mTyreFlags[(int)WheelIndex.TyreFrontLeft] & direDeflatedFlag) != direDeflatedFlag;
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.Detached = (data.mTyreFlags[(int)WheelIndex.TyreFrontRight] & direDeflatedFlag) != direDeflatedFlag;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.Detached = (data.mTyreFlags[(int)WheelIndex.TyreRearLeft] & direDeflatedFlag) != direDeflatedFlag;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.Detached = (data.mTyreFlags[(int)WheelIndex.TyreRearRight] & direDeflatedFlag) != direDeflatedFlag;

            /*simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RideHeight = Distance.FromMeters(data.mTyreHeightAboveGround[(int)WheelIndex.TyreFrontLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RideHeight = Distance.FromMeters(data.mTyreHeightAboveGround[(int)WheelIndex.TyreFrontRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RideHeight = Distance.FromMeters(data.mTyreHeightAboveGround[(int)WheelIndex.TyreRearLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RideHeight = Distance.FromMeters(data.mTyreHeightAboveGround[(int)WheelIndex.TyreRearRight]);*/

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreType = StringExtensions.FromArray(data.mLFTyreCompoundName);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreType = StringExtensions.FromArray(data.mRFTyreCompoundName);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreType = StringExtensions.FromArray(data.mLRTyreCompoundName);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreType = StringExtensions.FromArray(data.mRRTyreCompoundName);

            // Front Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreFrontLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreFrontLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreFrontLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreCoreTemperature.ActualQuantity = Temperature.FromKelvin(data.mTyreInternalAirTemp[(int)WheelIndex.TyreFrontLeft]);


            // Front Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreFrontRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreFrontRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreFrontRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreCoreTemperature.ActualQuantity = Temperature.FromKelvin(data.mTyreInternalAirTemp[(int)WheelIndex.TyreFrontRight]);


            // Rear Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreRearLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreRearLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreRearLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreCoreTemperature.ActualQuantity = Temperature.FromKelvin(data.mTyreInternalAirTemp[(int)WheelIndex.TyreRearLeft]);

            // Rear Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreRearRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreRearRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreRearRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreCoreTemperature.ActualQuantity = Temperature.FromKelvin(data.mTyreInternalAirTemp[(int)WheelIndex.TyreRearRight]);

            // Fuel System
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(data.mFuelCapacity);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining = Volume.FromLiters(data.mFuelLevel * data.mFuelCapacity);
        }

        private static void AddBrakesInfo(PCars2SharedMemory data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(data.mBrakeTempCelsius[(int)WheelIndex.TyreFrontLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(data.mBrakeTempCelsius[(int)WheelIndex.TyreFrontRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(data.mBrakeTempCelsius[(int)WheelIndex.TyreRearLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(data.mBrakeTempCelsius[(int)WheelIndex.TyreRearRight]);
        }

        private static void AddOilSystemInfo(PCars2SharedMemory data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.OilSystemInfo.OptimalOilTemperature.ActualQuantity = Temperature.FromCelsius(data.mOilTempCelsius);
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilPressure = Pressure.FromKiloPascals(data.mOilPressureKPa);
        }

        private static void AddWaterSystemInfo(PCars2SharedMemory data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WaterSystemInfo.OptimalWaterTemperature.ActualQuantity = Temperature.FromCelsius(data.mWaterTempCelsius);
            simData.PlayerInfo.CarInfo.WaterSystemInfo.WaterPressure = Pressure.FromKiloPascals(data.mWaterPressureKPa);
        }

        private static void AddPedalInfo(PCars2SharedMemory data, SimulatorDataSet simData)
        {
            simData.InputInfo.ThrottlePedalPosition = data.mUnfilteredThrottle;
            simData.InputInfo.BrakePedalPosition = data.mUnfilteredBrake;
            simData.InputInfo.ClutchPedalPosition = data.mUnfilteredClutch;
            simData.InputInfo.SteeringInput = data.mUnfilteredSteering;
        }


        private static void FillPlayerCarInfo(PCars2SharedMemory data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.EngineRpm = (int)data.mRpm;
            switch (data.mGear)
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
                    simData.PlayerInfo.CarInfo.CurrentGear = data.mGear.ToString();
                    break;
            }
        }

        internal void AddDriversData(SimulatorDataSet data, PCars2SharedMemory pcarsData)
        {
            if (pcarsData.mNumParticipants < 1)
            {
                return;
            }

            data.DriversInfo = new DriverInfo[pcarsData.mNumParticipants];
            DriverInfo playersInfo = null;

            for (int i = 0; i < pcarsData.mNumParticipants; i++)
            {
                ParticipantInfo pcVehicleInfo = pcarsData.mParticipantData[i];
                DriverInfo driverInfo = CreateDriverInfo(pcarsData, pcVehicleInfo, i);
                driverInfo.CurrentLapValid = pcarsData.mLapsInvalidated[i] == 0;
                data.DriversInfo[i] = driverInfo;

                if (driverInfo.IsPlayer)
                {
                    playersInfo = driverInfo;
                }

                if (driverInfo.Position == 1)
                {
                    data.SessionInfo.LeaderCurrentLap = driverInfo.CompletedLaps + 1;
                    data.LeaderInfo = driverInfo;
                }

                AddLappingInformation(data, pcarsData, driverInfo);
                FillTimingInfo(driverInfo, pcVehicleInfo, pcarsData, i);
            }

            _lastPlayer = playersInfo;

            if (playersInfo != null)
            {
                data.PlayerInfo = playersInfo;
            }
        }

        internal void FillTimingInfo(DriverInfo driverInfo, ParticipantInfo pcVehicleInfo, PCars2SharedMemory pCars2SharedMemory, int vehicleIndex)
        {
            driverInfo.Timing.LastSector1Time = CreateTimeSpan(pCars2SharedMemory.mCurrentSector1Times[vehicleIndex]);
            driverInfo.Timing.LastSector2Time = CreateTimeSpan(pCars2SharedMemory.mCurrentSector2Times[vehicleIndex]);
            driverInfo.Timing.LastSector3Time = CreateTimeSpan(pCars2SharedMemory.mCurrentSector3Times[vehicleIndex]);
            driverInfo.Timing.LastLapTime = CreateTimeSpan(pCars2SharedMemory.mLastLapTimes[vehicleIndex]);
            driverInfo.Timing.CurrentSector = pcVehicleInfo.mCurrentSector + 1;
            driverInfo.Timing.CurrentLapTime = TimeSpan.Zero;
        }

        private void AddLappingInformation(SimulatorDataSet data, PCars2SharedMemory pcarsData, DriverInfo driverInfo)
        {
            if (data.SessionInfo.SessionType == SessionType.Race && _lastPlayer != null
                && _lastPlayer.CompletedLaps != 0)
            {
                driverInfo.IsBeingLappedByPlayer = driverInfo.TotalDistance < (_lastPlayer.TotalDistance - (pcarsData.mTrackLength * 0.5));
                driverInfo.IsLappingPlayer = _lastPlayer.TotalDistance < (driverInfo.TotalDistance - (pcarsData.mTrackLength * 0.5));
            }
        }

        private DriverInfo CreateDriverInfo(PCars2SharedMemory pcarsData, ParticipantInfo pcVehicleInfo, int vehicleIndex)
        {
            DriverInfo driverInfo = new DriverInfo
            {
                DriverName = StringExtensions.FromArray(pcVehicleInfo.mName),
                CompletedLaps = (int) pcVehicleInfo.mLapsCompleted,
                CarName = StringExtensions.FromArray(pcarsData.mCarNames, vehicleIndex * 64),
                CarClassName = StringExtensions.FromArray(pcarsData.mCarClassNames, vehicleIndex * 64),
                InPits = pcarsData.mPitModes[vehicleIndex] != 0
            };

            driverInfo.CarClassId = driverInfo.CarClassName;
            driverInfo.IsPlayer = vehicleIndex == pcarsData.mViewedParticipantIndex;
            driverInfo.Position = (int)pcVehicleInfo.mRacePosition;
            driverInfo.Speed = Velocity.FromMs(pcarsData.mSpeeds[vehicleIndex]);
            driverInfo.LapDistance = pcVehicleInfo.mCurrentLapDistance;
            driverInfo.TotalDistance = (pcVehicleInfo.mLapsCompleted * pcarsData.mTrackLength) + driverInfo.LapDistance;
            driverInfo.FinishStatus = FromPCarStatus((RaceState)pcarsData.mRaceStates[vehicleIndex]);
            driverInfo.WorldPosition = new Point3D(Distance.FromMeters(-pcVehicleInfo.mWorldPosition[0]), Distance.FromMeters(pcVehicleInfo.mWorldPosition[1]), Distance.FromMeters(pcVehicleInfo.mWorldPosition[2]));
            ComputeDistanceToPlayer(_lastPlayer, driverInfo, pcarsData.mTrackLength);
            return driverInfo;
        }

        private static TimeSpan CreateTimeSpan(double seconds)
        {
            return seconds > 0 ? TimeSpan.FromSeconds(seconds) : TimeSpan.Zero;
        }

        private static DriverFinishStatus FromPCarStatus(RaceState finishStatus)
        {
            switch (finishStatus)
            {
                case RaceState.RaceStateInvalid:
                    return DriverFinishStatus.Na;

                case RaceState.RaceStateNotStarted:
                    return DriverFinishStatus.Dns;

                case RaceState.RaceStateRacing:
                    return DriverFinishStatus.None;

                case RaceState.RaceStateFinished:
                    return DriverFinishStatus.Finished;

                case RaceState.RaceStateDisqualified:
                    return DriverFinishStatus.Dnq;

                case RaceState.RaceStateRetired:
                    return DriverFinishStatus.Dnf;

                case RaceState.RaceStateDnf:
                    return DriverFinishStatus.Dnf;
                case RaceState.RaceStateMax:
                    return DriverFinishStatus.Na;
                default:
                    return DriverFinishStatus.Na;

            }
        }

        internal void FillSessionInfo(PCars2SharedMemory data, SimulatorDataSet simData, TimeSpan sessionTime)
        {
            // Timing
            simData.SessionInfo.SessionTime = sessionTime;
            simData.SessionInfo.TrackInfo.LayoutLength = Distance.FromMeters(data.mTrackLength);
            simData.SessionInfo.TrackInfo.TrackName = StringExtensions.FromArray(data.mTranslatedTrackLocation);
            simData.SessionInfo.TrackInfo.TrackLayoutName = StringExtensions.FromArray(data.mTranslatedTrackVariation);
            simData.SessionInfo.WeatherInfo.AirTemperature = Temperature.FromCelsius(data.mAmbientTemperature);
            simData.SessionInfo.WeatherInfo.TrackTemperature = Temperature.FromCelsius(data.mTrackTemperature);
            simData.SessionInfo.WeatherInfo.RainIntensity = (int) (Math.Max(data.mRainDensity, data.mSnowDensity) * 100.0);

            switch ((PCars2SessionType)data.mSessionState)
            {
                case PCars2SessionType.SessionInvalid:
                    simData.SessionInfo.SessionType = SessionType.Na;
                    break;
                case PCars2SessionType.SessionPractice:
                    simData.SessionInfo.SessionType = SessionType.Practice;
                    break;
                case PCars2SessionType.SessionTest:
                    simData.SessionInfo.SessionType = SessionType.Practice;
                    break;
                case PCars2SessionType.SessionQualify:
                    simData.SessionInfo.SessionType = SessionType.Qualification;
                    break;
                case PCars2SessionType.SessionFormationLap:
                    simData.SessionInfo.SessionType = SessionType.Race;
                    break;
                case PCars2SessionType.SessionRace:
                    simData.SessionInfo.SessionType = SessionType.Race;
                    break;
                case PCars2SessionType.SessionTimeAttack:
                    simData.SessionInfo.SessionType = SessionType.Practice;
                    break;
                case PCars2SessionType.SessionMax:
                    simData.SessionInfo.SessionType = SessionType.Na;
                    break;
            }

            switch ((GameState)data.mGameState)
            {
                case GameState.GameExited:
                    simData.SessionInfo.SessionPhase = SessionPhase.Countdown;
                    break;
                case GameState.GameFrontEnd:
                    simData.SessionInfo.SessionPhase = SessionPhase.Countdown;
                    break;
                case GameState.GameInGamePlaying:
                    simData.SessionInfo.SessionPhase = SessionPhase.Green;
                    break;
                case GameState.GameInGamePaused:
                    simData.SessionInfo.SessionPhase = SessionPhase.Green;
                    break;
                case GameState.GameInGameInMenuTimeTicking:
                    simData.SessionInfo.SessionPhase = SessionPhase.Green;
                    break;
                case GameState.GameInGameRestarting:
                    simData.SessionInfo.SessionPhase = SessionPhase.Checkered;
                    break;
                case GameState.GameInGameReplay:
                    simData.SessionInfo.SessionPhase = SessionPhase.Green;
                    break;
                case GameState.GameFrontEndReplay:
                    simData.SessionInfo.SessionPhase = SessionPhase.Green;
                    break;
                case GameState.GameMax:
                    break;
            }

            simData.SessionInfo.IsActive = simData.SessionInfo.SessionType != SessionType.Na;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (data.mEventTimeRemaining != -1)
            {
                simData.SessionInfo.SessionLengthType = SessionLengthType.Time;
                simData.SessionInfo.SessionTimeRemaining = data.mEventTimeRemaining;
            }
            else if (data.mLapsInEvent != 0)
            {
                simData.SessionInfo.SessionLengthType = SessionLengthType.Laps;
                simData.SessionInfo.TotalNumberOfLaps = (int)data.mLapsInEvent;
            }
        }


    }
}