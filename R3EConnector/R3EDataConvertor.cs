namespace SecondMonitor.R3EConnector
{
    using System;

    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;

    internal class R3EDataConvertor
    {

        private readonly R3RDatabase _database;
        private readonly R3EConnector _connector;

        private DriverInfo _lastPlayer;

        internal R3EDataConvertor(R3EConnector connector)
        {
            _connector = connector;
            _database = new R3RDatabase();
            _database.Load();

        }

        internal static void ComputeDistanceToPlayer(DriverInfo player, DriverInfo driverInfo, R3ESharedData r3RData)
        {
            if (player == null)
            {
                return;
            }

            if (driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Dq || driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Dnf ||
                driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Dnq || driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Dns)
            {
                driverInfo.DistanceToPlayer = double.MaxValue;
                return;
            }

            double trackLength = r3RData.LayoutLength;
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

        internal static DriverInfo.DriverFinishStatus FromR3RStatus(int finishStatus)
        {
            switch ((Constant.FinishStatus)finishStatus)
            {
                case Constant.FinishStatus.Unavailable:
                    return DriverInfo.DriverFinishStatus.Na;
                case Constant.FinishStatus.Dnf:
                    return DriverInfo.DriverFinishStatus.Dnf;
                case Constant.FinishStatus.Dnq:
                    return DriverInfo.DriverFinishStatus.Dnq;
                case Constant.FinishStatus.Dns:
                    return DriverInfo.DriverFinishStatus.Dns;
                case Constant.FinishStatus.Dq:
                    return DriverInfo.DriverFinishStatus.Dq;
                case Constant.FinishStatus.Finished:
                    return DriverInfo.DriverFinishStatus.Finished;
                case Constant.FinishStatus.None:
                    return DriverInfo.DriverFinishStatus.None;
                default:
                    return DriverInfo.DriverFinishStatus.Na;
            }
        }

        private static void AddAcceleration(R3ESharedData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.Acceleration.XinMs = data.LocalAcceleration.X;
            simData.PlayerInfo.CarInfo.Acceleration.YinMs = data.LocalAcceleration.Y;
            simData.PlayerInfo.CarInfo.Acceleration.ZinMs = data.LocalAcceleration.Z;
        }

        private static string FromByteArray(byte[] buffer)
        {
            if (buffer[0] == (char)0)
            {
                return string.Empty;
            }

            return System.Text.Encoding.UTF8.GetString(buffer).Split(new[] { (char)0 }, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        private static void AddTyresInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyrePressure.ActualQuantity = Pressure.FromKiloPascals(data.TirePressure.FrontLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyrePressure.ActualQuantity = Pressure.FromKiloPascals(data.TirePressure.FrontRight);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyrePressure.ActualQuantity = Pressure.FromKiloPascals(data.TirePressure.RearLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyrePressure.ActualQuantity = Pressure.FromKiloPascals(data.TirePressure.RearRight);



            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreWear = 1 - data.TireWear.FrontLeft;
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreWear = 1 - data.TireWear.FrontRight;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreWear = 1 - data.TireWear.RearLeft;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreWear = 1 - data.TireWear.RearRight;

            // Front Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.FrontLeft_Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.FrontLeft_Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.FrontLeft_Center);


            // Front Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.FrontRight_Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.FrontRight_Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.FrontRight_Center);


            // Rear Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.RearLeft_Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.RearLeft_Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.RearLeft_Center);

            // Rear Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.RearRight_Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.RearRight_Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.RearRight_Center);

            // Fuel System
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(data.FuelCapacity);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining = Volume.FromLiters(data.FuelLeft);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelPressure = Pressure.FromKiloPascals(data.FuelPressure);
        }

        private static void AddBrakesInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(data.BrakeTemp.FrontLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(data.BrakeTemp.FrontRight);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(data.BrakeTemp.RearLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(data.BrakeTemp.RearRight);
        }

        private static void AddOilSystemInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilPressure = Pressure.FromKiloPascals(data.EngineOilPressure);
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(data.EngineOilTemp);
        }

        private static void AddWaterSystemInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WaterSystemInfo.WaterTemperature = Temperature.FromCelsius(data.EngineWaterTemp);
        }

        private static void AddPedalInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            simData.PedalInfo.ThrottlePedalPosition = data.ThrottlePedal;
            simData.PedalInfo.BrakePedalPosition = data.BrakePedal;
            simData.PedalInfo.ClutchPedalPosition = data.ClutchPedal;
        }

        internal void AddDriversData(SimulatorDataSet data, R3ESharedData r3RData)
        {
            if (r3RData.NumCars == -1)
            {
                return;
            }

            data.DriversInfo = new DriverInfo[r3RData.NumCars];
            string playerName = FromByteArray(r3RData.PlayerName);
            DriverInfo playersInfo = null;

            for (int i = 0; i < r3RData.NumCars; i++)
            {
                DriverData r3RDriverData = r3RData.DriverData[i];
                DriverInfo driverInfo = CreateDriverInfo(r3RData, r3RDriverData, playerName);

                if (driverInfo.IsPlayer)
                {
                    playersInfo = driverInfo;
                    driverInfo.CurrentLapValid = r3RData.CurrentLapValid == 1;
                    _lastPlayer = driverInfo;
                }
                else
                {
                    driverInfo.CurrentLapValid = r3RDriverData.CurrentLapValid == 1;
                }

                AddWheelInfo(driverInfo, r3RDriverData);
                data.DriversInfo[i] = driverInfo;
                if (driverInfo.Position == 1)
                {
                    data.SessionInfo.LeaderCurrentLap = driverInfo.CompletedLaps + 1;
                    data.LeaderInfo = driverInfo;
                }

                AddLappingInformation(data, r3RData, driverInfo);
                FillTimingInfo(driverInfo, r3RDriverData, r3RData);

                if (driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Finished && !driverInfo.IsPlayer && driverInfo.Position > _lastPlayer?.Position)
                {
                    driverInfo.CompletedLaps--;
                    driverInfo.FinishStatus = DriverInfo.DriverFinishStatus.None;
                }

                _connector.StoreLastTickInfo(driverInfo);
            }

            if (playersInfo != null)
            {
                data.PlayerInfo = playersInfo;
            }
        }

        private static void AddWheelInfo(DriverInfo driverInfo, DriverData r3RDriverData)
        {
            driverInfo.CarInfo.WheelsInfo.FrontLeft.TyreType =
                ((Constant.TireSubtype)r3RDriverData.TireSubtypeFront).ToString();
            driverInfo.CarInfo.WheelsInfo.FrontLeft.TyreTypeFilled =
                ((Constant.TireSubtype)r3RDriverData.TireSubtypeFront) != Constant.TireSubtype.Unavailable;
            driverInfo.CarInfo.WheelsInfo.FrontRight.TyreType = driverInfo.CarInfo.WheelsInfo.FrontLeft.TyreType;
            driverInfo.CarInfo.WheelsInfo.FrontRight.TyreTypeFilled = driverInfo.CarInfo.WheelsInfo.FrontLeft.TyreTypeFilled;

            driverInfo.CarInfo.WheelsInfo.RearLeft.TyreTypeFilled =
                ((Constant.TireSubtype)r3RDriverData.TireSubtypeRear) != Constant.TireSubtype.Unavailable;
            driverInfo.CarInfo.WheelsInfo.RearLeft.TyreType = ((Constant.TireSubtype)r3RDriverData.TireSubtypeRear).ToString();
            driverInfo.CarInfo.WheelsInfo.RearRight.TyreType = driverInfo.CarInfo.WheelsInfo.RearLeft.TyreType;
            driverInfo.CarInfo.WheelsInfo.RearRight.TyreTypeFilled = driverInfo.CarInfo.WheelsInfo.RearLeft.TyreTypeFilled;
        }

        private void AddLappingInformation(SimulatorDataSet data, R3ESharedData r3RData, DriverInfo driverInfo)
        {
            if (data.SessionInfo.SessionType == SessionType.Race && _lastPlayer != null
                && _lastPlayer.CompletedLaps != 0)
            {
                driverInfo.IsBeingLappedByPlayer =
                    driverInfo.TotalDistance < (_lastPlayer.TotalDistance - r3RData.LayoutLength * 0.5);
                driverInfo.IsLappingPlayer =
                    _lastPlayer.TotalDistance < (driverInfo.TotalDistance - r3RData.LayoutLength * 0.5);
            }
        }

        private DriverInfo CreateDriverInfo(R3ESharedData r3RData, DriverData r3RDriverData, string playerName)
        {
            DriverInfo driverInfo = new DriverInfo
                                        {
                                            DriverName = FromByteArray(r3RDriverData.DriverInfo.Name),
                                            CompletedLaps = r3RDriverData.CompletedLaps,
                                            CarName = string.Empty,
                                            InPits = r3RDriverData.InPitlane == 1
                                        };

            driverInfo.IsPlayer = driverInfo.DriverName == playerName;
            driverInfo.Position = r3RDriverData.Place;
            driverInfo.Speed = Velocity.FromMs(r3RDriverData.CarSpeed);
            driverInfo.LapDistance = r3RDriverData.LapDistance;
            driverInfo.TotalDistance = r3RDriverData.CompletedLaps * r3RData.LayoutLength + r3RDriverData.LapDistance;
            driverInfo.CarName = _database.GetCarName(r3RDriverData.DriverInfo.ModelId);
            driverInfo.FinishStatus = FromR3RStatus(r3RDriverData.FinishStatus);
            driverInfo.WorldPosition = new Point3D(Distance.FromMeters(r3RDriverData.Position.X), Distance.FromMeters(r3RDriverData.Position.Y), Distance.FromMeters(r3RDriverData.Position.Z));
            ComputeDistanceToPlayer(_lastPlayer, driverInfo, r3RData);
            return driverInfo;
        }

        internal void FillTimingInfo(DriverInfo driverInfo, DriverData r3EDriverData, R3ESharedData r3RData)
        {
            if (driverInfo.IsPlayer)
            {
                driverInfo.Timing.LastLapTime = r3RData.LapTimePreviousSelf != -1 ? TimeSpan.FromSeconds(r3RData.LapTimePreviousSelf) : TimeSpan.Zero;
                if (r3RData.SectorTimesCurrentSelf.Sector1 != -1 || r3RData.SectorTimesCurrentSelf.Sector2 != -1
                    || r3RData.SectorTimesPreviousSelf.Sector3 != -1)
                {
                    driverInfo.Timing.LastSector1Time = TimeSpan.FromSeconds(r3RData.SectorTimesCurrentSelf.Sector1);
                    driverInfo.Timing.LastSector2Time = TimeSpan.FromSeconds(r3RData.SectorTimesCurrentSelf.Sector2 - r3RData.SectorTimesCurrentSelf.Sector1);
                    driverInfo.Timing.LastSector3Time = TimeSpan.FromSeconds(r3RData.SectorTimesPreviousSelf.Sector3 - r3RData.SectorTimesPreviousSelf.Sector2);
                }
                driverInfo.Timing.CurrentSector = r3EDriverData.TrackSector;
                driverInfo.Timing.CurrentLapTime = TimeSpan.FromSeconds(r3RData.LapTimeCurrentSelf);
                return;
            }

            if (r3EDriverData.SectorTimesCurrentSelf.Sector1 != -1 || r3EDriverData.SectorTimesCurrentSelf.Sector2 != -1
                || r3EDriverData.SectorTimesPreviousSelf.Sector3 != -1)
            {
                driverInfo.Timing.LastSector1Time = TimeSpan.FromSeconds(r3EDriverData.SectorTimesCurrentSelf.Sector1);
                driverInfo.Timing.LastSector2Time = TimeSpan.FromSeconds(r3EDriverData.SectorTimesCurrentSelf.Sector2 - r3EDriverData.SectorTimesCurrentSelf.Sector1);
                driverInfo.Timing.LastSector3Time = TimeSpan.FromSeconds(r3EDriverData.SectorTimesPreviousSelf.Sector3 - r3EDriverData.SectorTimesPreviousSelf.Sector2);
            }

            driverInfo.Timing.CurrentLapTime = TimeSpan.FromSeconds(r3EDriverData.LapTimeCurrentSelf);
            driverInfo.Timing.LastLapTime = r3EDriverData.SectorTimesPreviousSelf.Sector3 != -1 ? TimeSpan.FromSeconds(r3EDriverData.SectorTimesPreviousSelf.Sector3) : TimeSpan.Zero;
            driverInfo.Timing.CurrentSector = r3EDriverData.TrackSector;
        }

        internal SimulatorDataSet FromR3EData(R3ESharedData data)
        {
            SimulatorDataSet simData = new SimulatorDataSet("R3E");
            simData.SimulatorSourceInfo.HasLapTimeInformation = true;
            simData.SimulatorSourceInfo.SectorTimingSupport = DataInputSupport.FULL;
            simData.SimulatorSourceInfo.AIInstantFinish = true;

            // SimulatorDataSet simData = new SimulatorDataSet("R3R");
            FillSessionInfo(data, simData);
            AddDriversData(simData, data);

            FillPlayersGear(data, simData);

            // PEDAL INFO
            AddPedalInfo(data, simData);

            // WaterSystemInfo
            AddWaterSystemInfo(data, simData);

            // OilSystemInfo
            AddOilSystemInfo(data, simData);

            // Brakes Info
            AddBrakesInfo(data, simData);

            // Tyre Pressure Info
            AddTyresInfo(data, simData);

            // Acceleration
            AddAcceleration(data, simData);

            return simData;
        }

        private static void FillPlayersGear(R3ESharedData data, SimulatorDataSet simData)
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
                    simData.PlayerInfo.CarInfo.CurrentGear = string.Empty;
                    break;
                default:
                    simData.PlayerInfo.CarInfo.CurrentGear = data.Gear.ToString();
                    break;
            }
        }

        internal void FillSessionInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            // Timing
            simData.SessionInfo.SessionTime = _connector.SessionTime; // TimeSpan.FromSeconds(data.Player.GameSimulationTime);
            simData.SessionInfo.TrackInfo.LayoutLength = data.LayoutLength;
            simData.SessionInfo.IsActive = (Constant.SessionPhase)data.SessionPhase == Constant.SessionPhase.Green
                || (Constant.SessionPhase)data.SessionPhase == Constant.SessionPhase.Checkered;
            switch ((Constant.Session)data.SessionType)
            {
                case Constant.Session.Practice:
                    simData.SessionInfo.SessionType = SessionType.Practice;
                    break;
                case Constant.Session.Qualify:
                    simData.SessionInfo.SessionType = SessionType.Qualification;
                    break;
                case Constant.Session.Race:
                    simData.SessionInfo.SessionType = SessionType.Race;
                    break;
                case Constant.Session.Unavailable:
                    simData.SessionInfo.SessionType = SessionType.Na;
                    break;
                case Constant.Session.Warmup:
                    simData.SessionInfo.SessionType = SessionType.WarmUp;
                    break;
                default:
                    simData.SessionInfo.SessionType = SessionType.Na;
                    break;
            }

            switch ((Constant.SessionPhase)data.SessionPhase)
            {
                case Constant.SessionPhase.Countdown:
                case Constant.SessionPhase.Formation:
                case Constant.SessionPhase.Gridwalk:
                case Constant.SessionPhase.Garage:
                    simData.SessionInfo.SessionPhase = SessionPhase.Countdown;
                    break;
                case Constant.SessionPhase.Green:
                    simData.SessionInfo.SessionPhase = SessionPhase.Green;
                    break;
                case Constant.SessionPhase.Checkered:
                    simData.SessionInfo.SessionPhase = SessionPhase.Checkered;
                    break;
                default:
                    simData.SessionInfo.SessionPhase = SessionPhase.Green;
                    break;

            }

            simData.SessionInfo.TrackInfo.TrackName = FromByteArray(data.TrackName);
            simData.SessionInfo.TrackInfo.TrackLayoutName = FromByteArray(data.LayoutName);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (data.SessionTimeRemaining != -1)
            {
                simData.SessionInfo.SessionLengthType = SessionLengthType.Time;
                simData.SessionInfo.SessionTimeRemaining = data.SessionTimeRemaining;
            }
            else if (data.NumberOfLaps != -1)
            {
                simData.SessionInfo.SessionLengthType = SessionLengthType.Laps;
                simData.SessionInfo.TotalNumberOfLaps = data.NumberOfLaps;
            }
        }
    }
}
