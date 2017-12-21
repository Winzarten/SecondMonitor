namespace SecondMonitor.R3EConnector
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using SecondMonitor.DataModel;
    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Drivers;
   

    internal class R3EDataConvertor
    {

        private readonly R3RDatabase _database;
        private readonly R3EConnector _connector;
        private DriverInfo _lastPlayer = new DriverInfo();        
        
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
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.FrontLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.FrontRight);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.RearLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.RearRight);



            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreWear = 1 - data.TireWear.FrontRight;
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreWear = 1 - data.TireWear.FrontRight;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreWear = 1 - data.TireWear.RearLeft;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreWear = 1 - data.TireWear.RearRight;

            // Front Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.LeftTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontLeft_Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RightTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontLeft_Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.CenterTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontLeft_Center);


            // Front Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.LeftTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontRight_Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RightTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontRight_Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.CenterTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontRight_Center);


            // Rear Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.LeftTyreTemp = Temperature.FromCelsius(data.TireTemp.RearLeft_Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RightTyreTemp = Temperature.FromCelsius(data.TireTemp.RearLeft_Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.CenterTyreTemp = Temperature.FromCelsius(data.TireTemp.RearLeft_Center);

            // Rear Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.LeftTyreTemp = Temperature.FromCelsius(data.TireTemp.RearRight_Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RightTyreTemp = Temperature.FromCelsius(data.TireTemp.RearRight_Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.CenterTyreTemp = Temperature.FromCelsius(data.TireTemp.RearRight_Center);

            // Fuel System
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(data.FuelCapacity);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining = Volume.FromLiters(data.FuelLeft);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelPressure = Pressure.FromKiloPascals(data.FuelPressure);
        }

        private static void AddBrakesInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature = Temperature.FromCelsius(data.BrakeTemp.FrontLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.BrakeTemperature = Temperature.FromCelsius(data.BrakeTemp.FrontRight);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.BrakeTemperature = Temperature.FromCelsius(data.BrakeTemp.RearLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.BrakeTemperature = Temperature.FromCelsius(data.BrakeTemp.RearRight);
        }

        private static void AddOilSystemInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilPressure = Pressure.FromKiloPascals(data.EngineOilPressure);
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(data.EngineOilTemp);
        }

        private static void AddWaterSystemInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WaterSystmeInfo.WaterTemperature = Temperature.FromCelsius(data.EngineWaterTemp);
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
                FillTimingInfor(driverInfo, r3RDriverData, r3RData);

                if (driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Finished)
                {
                    driverInfo.CompletedLaps++;
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
            driverInfo.CarInfo.WheelsInfo.FrontRight.TyreTypeFilled =
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
            if (data.SessionInfo.SessionType == SessionInfo.SessionTypeEnum.Race && _lastPlayer != null
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
            ComputeDistanceToPlayer(_lastPlayer, driverInfo, r3RData);
            return driverInfo;
        }
       
        internal void FillTimingInfor(DriverInfo driverInfo, DriverData r3EDriverData, R3ESharedData r3RData)
        {
            if (driverInfo.IsPlayer)
            {
                driverInfo.Timing.LastLapTime = r3RData.LapTimePreviousSelf;
                return;
            }

            driverInfo.Timing.LastLapTime = r3EDriverData.SectorTimePreviousSelf.Sector3;
        }
        
        internal SimulatorDataSet FromR3EData(R3ESharedData data)
        {
            SimulatorDataSet simData = new SimulatorDataSet("R3R");
            simData.SimulatorSourceInfo.HasLapTimeInformation = true;

            // SimulatorDataSet simData = new SimulatorDataSet("R3R");
            FillSessionInfo(data, simData);
            AddDriversData(simData, data);

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
        
        internal void FillSessionInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            // Timing
            simData.SessionInfo.SessionTime = _connector.SessionTime; // TimeSpan.FromSeconds(data.Player.GameSimulationTime);
            simData.SessionInfo.LayoutLength = data.LayoutLength;
            simData.SessionInfo.IsActive = (Constant.SessionPhase)data.SessionPhase == Constant.SessionPhase.Green
                || (Constant.SessionPhase)data.SessionPhase == Constant.SessionPhase.Checkered;
            switch ((Constant.Session)data.SessionType)
            {
                case Constant.Session.Practice:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.Practice;
                    break;
                case Constant.Session.Qualify:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.Qualification;
                    break;
                case Constant.Session.Race:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.Race;
                    break;
                case Constant.Session.Unavailable:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.Na;
                    break;
                case Constant.Session.Warmup:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.WarmUp;
                    break;
                default:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.Na;
                    break;
            }

            switch ((Constant.SessionPhase)data.SessionPhase)
            {
                case Constant.SessionPhase.Countdown:
                case Constant.SessionPhase.Formation:
                case Constant.SessionPhase.Gridwalk:
                case Constant.SessionPhase.Garage:
                    simData.SessionInfo.SessionPhase = SessionInfo.SessionPhaseEnum.Countdown;
                    break;
                case Constant.SessionPhase.Green:
                    simData.SessionInfo.SessionPhase = SessionInfo.SessionPhaseEnum.Green;
                    break;
                case Constant.SessionPhase.Checkered:
                    simData.SessionInfo.SessionPhase = SessionInfo.SessionPhaseEnum.Checkered;
                    break;
                default:
                    simData.SessionInfo.SessionPhase = SessionInfo.SessionPhaseEnum.Green;
                    break;

            }

            simData.SessionInfo.TrackName = FromByteArray(data.TrackName);
            simData.SessionInfo.TrackLayoutName = FromByteArray(data.LayoutName);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (data.SessionTimeRemaining != -1)
            {
                simData.SessionInfo.SessionLengthType = SessionInfo.SessionLengthTypeEnum.Time;
                simData.SessionInfo.SessionTimeRemaining = data.SessionTimeRemaining;
            }
            else if (data.NumberOfLaps != -1)
            {
                simData.SessionInfo.SessionLengthType = SessionInfo.SessionLengthTypeEnum.Laps;
                simData.SessionInfo.TotalNumberOfLaps = data.NumberOfLaps;
            }
        }       
    }
}
