namespace SecondMonitor.PCars2Connector.DataConvertor
{
    using System;

    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;
    using SharedMemory;
    using PluginManager.Extensions;

    public class PCars2DataConvertor
    {
        private DriverInfo _lastPlayer = new DriverInfo();

        public SimulatorDataSet CreateSimulatorDataSet(PCars2SharedMemory pcarsData, TimeSpan sessionTime)
        {
            SimulatorDataSet simData = new SimulatorDataSet("PCars 2");
            simData.SimulatorSourceInfo.HasLapTimeInformation = true;
            simData.SimulatorSourceInfo.OutLapIsValid = true;
            simData.SimulatorSourceInfo.InvalidateLapBySector = true;
            simData.SimulatorSourceInfo.SectorTimingSupport = DataInputSupport.Full;

            FillSessionInfo(pcarsData, simData, sessionTime);
            AddDriversData(simData, pcarsData);

            FillPlayersGear(pcarsData, simData);

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

            // Acceleration
            AddAcceleration(pcarsData, simData);

            if (simData.PlayerInfo?.FinishStatus == DriverFinishStatus.Dns && simData.SessionInfo.SessionType == SessionType.Race)
            {
                simData.SessionInfo.SessionPhase = SessionPhase.Countdown;
            }

            return simData;
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

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreType = StringExtensions.FromArray(data.mLFTyreCompoundName);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreType = StringExtensions.FromArray(data.mRFTyreCompoundName);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreType = StringExtensions.FromArray(data.mLRTyreCompoundName);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreType = StringExtensions.FromArray(data.mRRTyreCompoundName);

            // Front Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreFrontLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreFrontLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreFrontLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreCoreTemperature.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreFrontLeft]);


            // Front Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreFrontRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreFrontRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreFrontRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreCoreTemperature.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreFrontRight]);


            // Rear Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreRearLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreRearLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreRearLeft]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreCoreTemperature.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreRearLeft]);

            // Rear Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreRearRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreRearRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreRearRight]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreCoreTemperature.ActualQuantity = Temperature.FromCelsius(data.mTyreTemp[(int)WheelIndex.TyreRearRight]);

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
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(data.mOilTempCelsius);
        }

        private static void AddWaterSystemInfo(PCars2SharedMemory data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WaterSystemInfo.WaterTemperature = Temperature.FromCelsius(data.mWaterTempCelsius);
        }

        private static void AddPedalInfo(PCars2SharedMemory data, SimulatorDataSet simData)
        {
            simData.PedalInfo.ThrottlePedalPosition = data.mUnfilteredThrottle;
            simData.PedalInfo.BrakePedalPosition = data.mUnfilteredBrake;
            simData.PedalInfo.ClutchPedalPosition = data.mUnfilteredClutch;
        }


        private static void FillPlayersGear(PCars2SharedMemory data, SimulatorDataSet simData)
        {
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
                InPits = pcarsData.mPitModes[vehicleIndex] != 0
            };

            driverInfo.IsPlayer = vehicleIndex == pcarsData.mViewedParticipantIndex;
            driverInfo.Position = (int)pcVehicleInfo.mRacePosition;
            driverInfo.Speed = Velocity.FromMs(pcarsData.mSpeeds[vehicleIndex]);
            driverInfo.LapDistance = pcVehicleInfo.mCurrentLapDistance;
            driverInfo.TotalDistance = (pcVehicleInfo.mLapsCompleted * pcarsData.mTrackLength) + driverInfo.LapDistance;
            driverInfo.FinishStatus = FromPCarStatus((RaceState)pcarsData.mRaceStates[vehicleIndex]);
            driverInfo.WorldPosition = new Point3D(Distance.FromMeters(-pcVehicleInfo.mWorldPosition[0]), Distance.FromMeters(pcVehicleInfo.mWorldPosition[1]), Distance.FromMeters(pcVehicleInfo.mWorldPosition[2]));
            ComputeDistanceToPlayer(_lastPlayer, driverInfo, pcarsData);
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


        internal static void ComputeDistanceToPlayer(DriverInfo player, DriverInfo driverInfo, PCars2SharedMemory PCars2SharedMemory)
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

            double trackLength = PCars2SharedMemory.mTrackLength;
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

        internal void FillSessionInfo(PCars2SharedMemory data, SimulatorDataSet simData, TimeSpan sessionTime)
        {
            // Timing
            simData.SessionInfo.SessionTime = sessionTime;
            simData.SessionInfo.TrackInfo.LayoutLength = Distance.FromMeters(data.mTrackLength);
            simData.SessionInfo.TrackInfo.TrackName = StringExtensions.FromArray(data.mTranslatedTrackLocation);
            simData.SessionInfo.TrackInfo.TrackLayoutName = StringExtensions.FromArray(data.mTranslatedTrackVariation);
            simData.SessionInfo.WeatherInfo.AirTemperature = Temperature.FromCelsius(data.mAmbientTemperature);
            simData.SessionInfo.WeatherInfo.TrackTemperature = Temperature.FromCelsius(data.mTrackTemperature);
            simData.SessionInfo.WeatherInfo.RainIntensity = (int) (data.mRainDensity * 100.0);

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