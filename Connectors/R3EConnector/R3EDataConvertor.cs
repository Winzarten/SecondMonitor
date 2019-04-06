namespace SecondMonitor.R3EConnector
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;
    using DataModel.Snapshot.Systems;
    using PluginManager.GameConnector;
    using DriverInfo = DataModel.Snapshot.Drivers.DriverInfo;

    internal class R3EDataConvertor : AbstractDataConvertor
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

            if (driverInfo.FinishStatus == DriverFinishStatus.Dq || driverInfo.FinishStatus == DriverFinishStatus.Dnf ||
                driverInfo.FinishStatus == DriverFinishStatus.Dnq || driverInfo.FinishStatus == DriverFinishStatus.Dns)
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

        internal static DriverFinishStatus FromR3RStatus(int finishStatus)
        {
            switch ((Constant.FinishStatus)finishStatus)
            {
                case Constant.FinishStatus.Unavailable:
                    return DriverFinishStatus.Na;
                case Constant.FinishStatus.Dnf:
                    return DriverFinishStatus.Dnf;
                case Constant.FinishStatus.Dnq:
                    return DriverFinishStatus.Dnq;
                case Constant.FinishStatus.Dns:
                    return DriverFinishStatus.Dns;
                case Constant.FinishStatus.Dq:
                    return DriverFinishStatus.Dq;
                case Constant.FinishStatus.Finished:
                    return DriverFinishStatus.Finished;
                case Constant.FinishStatus.None:
                    return DriverFinishStatus.None;
                default:
                    return DriverFinishStatus.Na;
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

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreWear.ActualWear = 1 - data.TireWear.FrontLeft;
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreWear.ActualWear = 1 - data.TireWear.FrontRight;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreWear.ActualWear = 1 - data.TireWear.RearLeft;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreWear.ActualWear = 1 - data.TireWear.RearRight;

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.Rps = -data.TireRps.FrontLeft;
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.Rps = -data.TireRps.FrontRight;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.Rps = -data.TireRps.RearLeft;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.Rps = -data.TireRps.RearRight;

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.DirtLevel = data.TireDirt.FrontLeft;
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.DirtLevel = data.TireDirt.FrontRight;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.DirtLevel = data.TireDirt.RearLeft;
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.DirtLevel = data.TireDirt.RearRight;

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.SuspensionTravel = Distance.FromMeters(data.Player.SuspensionDeflection.FrontLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.SuspensionTravel = Distance.FromMeters(data.Player.SuspensionDeflection.FrontRight);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.SuspensionTravel = Distance.FromMeters(data.Player.SuspensionDeflection.RearLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.SuspensionTravel = Distance.FromMeters(data.Player.SuspensionDeflection.RearRight);

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.SuspensionVelocity = Velocity.FromMs(data.Player.SuspensionVelocity.FrontLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.SuspensionVelocity = Velocity.FromMs(data.Player.SuspensionVelocity.FrontRight);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.SuspensionVelocity = Velocity.FromMs(data.Player.SuspensionVelocity.RearLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.SuspensionVelocity = Velocity.FromMs(data.Player.SuspensionVelocity.RearRight);

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.Camber = Angle.GetFromRadians(data.Player.Camber.FrontLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.Camber = Angle.GetFromRadians(data.Player.Camber.FrontRight);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.Camber = Angle.GetFromRadians(data.Player.Camber.RearLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.Camber = Angle.GetFromRadians(data.Player.Camber.RearRight);

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RideHeight = Distance.FromMeters(data.Player.RideHeight.FrontLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RideHeight = Distance.FromMeters(data.Player.RideHeight.FrontRight);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RideHeight = Distance.FromMeters(data.Player.RideHeight.RearLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RideHeight = Distance.FromMeters(data.Player.RideHeight.RearRight);

            // Front Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.CenterTyreTemp.IdealQuantity = Temperature.FromCelsius(data.TireTemp.FrontLeft.OptimalTemp);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.CenterTyreTemp.IdealQuantityWindow = Temperature.FromCelsius((data.TireTemp.FrontLeft.HotTemp - data.TireTemp.FrontLeft.OptimalTemp) * 0.5);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.FrontLeft.CurrentTemp.Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.FrontLeft.CurrentTemp.Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.FrontLeft.CurrentTemp.Center);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreCoreTemperature.ActualQuantity = Temperature.FromCelsius((data.TireTemp.FrontLeft.CurrentTemp.Left + data.TireTemp.FrontLeft.CurrentTemp.Center + data.TireTemp.FrontLeft.CurrentTemp.Right) / 3);


            // Front Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.CenterTyreTemp.IdealQuantity = Temperature.FromCelsius(data.TireTemp.FrontLeft.OptimalTemp);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.CenterTyreTemp.IdealQuantityWindow = Temperature.FromCelsius((data.TireTemp.FrontLeft.HotTemp - data.TireTemp.FrontLeft.OptimalTemp) * 0.5);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.FrontRight.CurrentTemp.Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.FrontRight.CurrentTemp.Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.FrontRight.CurrentTemp.Center);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreCoreTemperature.ActualQuantity = Temperature.FromCelsius((data.TireTemp.FrontRight.CurrentTemp.Left + data.TireTemp.FrontRight.CurrentTemp.Center + data.TireTemp.FrontRight.CurrentTemp.Right) / 3);


            // Rear Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.CenterTyreTemp.IdealQuantity = Temperature.FromCelsius(data.TireTemp.FrontLeft.OptimalTemp);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.CenterTyreTemp.IdealQuantityWindow = Temperature.FromCelsius((data.TireTemp.FrontLeft.HotTemp - data.TireTemp.FrontLeft.OptimalTemp) * 0.5);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.RearLeft.CurrentTemp.Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.RearLeft.CurrentTemp.Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.RearLeft.CurrentTemp.Center);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreCoreTemperature.ActualQuantity = Temperature.FromCelsius((data.TireTemp.RearLeft.CurrentTemp.Left + data.TireTemp.RearLeft.CurrentTemp.Center + data.TireTemp.RearLeft.CurrentTemp.Right) / 3);

            // Rear Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.CenterTyreTemp.IdealQuantity = Temperature.FromCelsius(data.TireTemp.FrontLeft.OptimalTemp);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.CenterTyreTemp.IdealQuantityWindow = Temperature.FromCelsius((data.TireTemp.FrontLeft.HotTemp - data.TireTemp.FrontLeft.OptimalTemp) * 0.5);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.RearRight.CurrentTemp.Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.RearRight.CurrentTemp.Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(data.TireTemp.RearRight.CurrentTemp.Center);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreCoreTemperature.ActualQuantity = Temperature.FromCelsius((data.TireTemp.RearRight.CurrentTemp.Left + data.TireTemp.RearRight.CurrentTemp.Center + data.TireTemp.RearRight.CurrentTemp.Right) / 3);

            // Fuel System
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(data.FuelCapacity);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining = Volume.FromLiters(data.FuelLeft);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelPressure = Pressure.FromKiloPascals(data.FuelPressure);
        }

        private static void AddBrakesInfo(R3ESharedData data, SimulatorDataSet simData)
        {

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(data.BrakeTemp.FrontLeft.CurrentTemp);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature.IdealQuantityWindow = Temperature.FromCelsius((data.BrakeTemp.FrontLeft.HotTemp - data.BrakeTemp.FrontLeft.OptimalTemp) / 2);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature.IdealQuantity = Temperature.FromCelsius(data.BrakeTemp.FrontLeft.OptimalTemp);

            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(data.BrakeTemp.FrontRight.CurrentTemp);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.BrakeTemperature.IdealQuantityWindow = Temperature.FromCelsius((data.BrakeTemp.FrontRight.HotTemp - data.BrakeTemp.FrontRight.OptimalTemp) / 2);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.BrakeTemperature.IdealQuantity = Temperature.FromCelsius(data.BrakeTemp.FrontRight.OptimalTemp);

            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(data.BrakeTemp.RearLeft.CurrentTemp);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.BrakeTemperature.IdealQuantityWindow = Temperature.FromCelsius((data.BrakeTemp.RearLeft.HotTemp - data.BrakeTemp.RearLeft.OptimalTemp) / 2);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.BrakeTemperature.IdealQuantity = Temperature.FromCelsius(data.BrakeTemp.RearLeft.OptimalTemp);

            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(data.BrakeTemp.RearRight.CurrentTemp);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.BrakeTemperature.IdealQuantityWindow = Temperature.FromCelsius((data.BrakeTemp.RearRight.HotTemp - data.BrakeTemp.RearRight.OptimalTemp) / 2);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.BrakeTemperature.IdealQuantity = Temperature.FromCelsius(data.BrakeTemp.RearRight.OptimalTemp);
        }

        private static void AddOilSystemInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilPressure = Pressure.FromKiloPascals(data.EngineOilPressure);
            simData.PlayerInfo.CarInfo.OilSystemInfo.OptimalOilTemperature.ActualQuantity = Temperature.FromCelsius(data.EngineOilTemp);
        }

        private static void AddWaterSystemInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.WaterSystemInfo.OptimalWaterTemperature.ActualQuantity = Temperature.FromCelsius(data.EngineWaterTemp);
        }

        private static void AddPedalInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            simData.InputInfo.ThrottlePedalPosition = data.ThrottleRaw;
            simData.InputInfo.BrakePedalPosition = data.BrakeRaw;
            simData.InputInfo.ClutchPedalPosition = data.ClutchRaw;
            simData.InputInfo.SteeringInput = data.SteerInputRaw;
            simData.InputInfo.WheelAngle = data.SteerWheelRangeDegrees * data.SteerInputRaw;
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

                if (driverInfo.FinishStatus == DriverFinishStatus.Finished && !driverInfo.IsPlayer && driverInfo.Position > _lastPlayer?.Position)
                {
                    driverInfo.CompletedLaps--;
                    driverInfo.FinishStatus = DriverFinishStatus.None;
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
            driverInfo.CarInfo.WheelsInfo.FrontLeft.TyreType = ((Constant.TireSubtype)r3RDriverData.TireSubtypeFront).ToString();
            driverInfo.CarInfo.WheelsInfo.FrontRight.TyreType = driverInfo.CarInfo.WheelsInfo.FrontLeft.TyreType;

            driverInfo.CarInfo.WheelsInfo.RearLeft.TyreType = ((Constant.TireSubtype)r3RDriverData.TireSubtypeRear).ToString();
            driverInfo.CarInfo.WheelsInfo.RearRight.TyreType = driverInfo.CarInfo.WheelsInfo.RearLeft.TyreType;

            if (driverInfo.CarInfo.WheelsInfo.FrontLeft.TyreType == "Unavailable")
            {
                driverInfo.CarInfo.WheelsInfo.FrontLeft.TyreType = "Prime";
                driverInfo.CarInfo.WheelsInfo.FrontRight.TyreType = "Prime";
            }

            if (driverInfo.CarInfo.WheelsInfo.RearLeft.TyreType == "Unavailable")
            {
                driverInfo.CarInfo.WheelsInfo.RearLeft.TyreType = "Prime";
                driverInfo.CarInfo.WheelsInfo.RearRight.TyreType = "Prime";
            }

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
                                            DriverName = FromByteArray(r3RDriverData.R3EDriverInfo.Name),
                                            CompletedLaps = r3RDriverData.CompletedLaps,
                                            CarName = string.Empty,
                                            InPits = r3RDriverData.InPitlane == 1
                                        };

            driverInfo.IsPlayer = driverInfo.DriverName == playerName;
            driverInfo.Position = r3RDriverData.Place;
            driverInfo.PositionInClass = r3RDriverData.PlaceClass;
            driverInfo.Speed = Velocity.FromMs(r3RDriverData.CarSpeed);
            driverInfo.LapDistance = r3RDriverData.LapDistance;
            driverInfo.TotalDistance = r3RDriverData.CompletedLaps * r3RData.LayoutLength + r3RDriverData.LapDistance;
            driverInfo.CarName = _database.GetCarName(r3RDriverData.R3EDriverInfo.ModelId);
            driverInfo.CarClassName = _database.GetClassName(r3RDriverData.R3EDriverInfo.ClassId);
            driverInfo.CarClassId = r3RDriverData.R3EDriverInfo.ClassPerformanceIndex.ToString();
            driverInfo.FinishStatus = FromR3RStatus(r3RDriverData.FinishStatus);
            driverInfo.WorldPosition = new Point3D(Distance.FromMeters(r3RDriverData.Position.X * -1), Distance.FromMeters(r3RDriverData.Position.Y), Distance.FromMeters(r3RDriverData.Position.Z));
            ComputeDistanceToPlayer(_lastPlayer, driverInfo, r3RData);
            return driverInfo;
        }

        internal void FillTimingInfo(DriverInfo driverInfo, DriverData r3EDriverData, R3ESharedData r3RData)
        {
            driverInfo.Timing.GapAhead = TimeSpan.FromSeconds(r3EDriverData.TimeDeltaFront);
            driverInfo.Timing.GapBehind = TimeSpan.FromSeconds(r3EDriverData.TimeDeltaBehind);

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

            if (r3EDriverData.SectorTimeCurrentSelf.Sector1 != -1 || r3EDriverData.SectorTimeCurrentSelf.Sector2 != -1
                || r3EDriverData.SectorTimePreviousSelf.Sector3 != -1)
            {
                driverInfo.Timing.LastSector1Time = TimeSpan.FromSeconds(r3EDriverData.SectorTimeCurrentSelf.Sector1);
                driverInfo.Timing.LastSector2Time = TimeSpan.FromSeconds(r3EDriverData.SectorTimeCurrentSelf.Sector2 - r3EDriverData.SectorTimeCurrentSelf.Sector1);
                driverInfo.Timing.LastSector3Time = TimeSpan.FromSeconds(r3EDriverData.SectorTimePreviousSelf.Sector3 - r3EDriverData.SectorTimePreviousSelf.Sector2);
            }

            driverInfo.Timing.CurrentLapTime = TimeSpan.FromSeconds(r3EDriverData.LapTimeCurrentSelf);
            driverInfo.Timing.LastLapTime = r3EDriverData.SectorTimePreviousSelf.Sector3 != -1 ? TimeSpan.FromSeconds(r3EDriverData.SectorTimePreviousSelf.Sector3) : TimeSpan.Zero;
            driverInfo.Timing.CurrentSector = r3EDriverData.TrackSector;
        }

        internal SimulatorDataSet FromR3EData(R3ESharedData data)
        {
            SimulatorDataSet simData = new SimulatorDataSet("R3E");
            simData.SimulatorSourceInfo.HasLapTimeInformation = true;
            simData.SimulatorSourceInfo.SectorTimingSupport = DataInputSupport.Full;
            simData.SimulatorSourceInfo.AIInstantFinish = true;
            simData.SimulatorSourceInfo.GapInformationProvided = GapInformationKind.TimeToSurroundingDrivers;
            simData.SimulatorSourceInfo.TelemetryInfo.ContainsSuspensionVelocity = true;
            simData.SimulatorSourceInfo.TelemetryInfo.ContainsOptimalTemperatures = true;

            // SimulatorDataSet simData = new SimulatorDataSet("R3R");
            FillSessionInfo(data, simData);
            AddDriversData(simData, data);

            FillPlayerCarInfo(data, simData);

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

            //Add Additional Player Car Info
            AddPlayerCarInfo(data, simData);

            //Add Flags Info
            AddFlags(data, simData);

            return simData;
        }

        private void AddPlayerCarInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            CarInfo playerCar = simData.PlayerInfo.CarInfo;

            playerCar.CarDamageInformation.Bodywork.MediumDamageThreshold = 0.01;
            playerCar.CarDamageInformation.Engine.MediumDamageThreshold = 0.01;
            playerCar.CarDamageInformation.Transmission.MediumDamageThreshold = 0.01;
            playerCar.CarDamageInformation.Suspension.Damage = 1 - data.CarDamage.Suspension;
            playerCar.CarDamageInformation.Bodywork.Damage = 1 - data.CarDamage.Aerodynamics;
            playerCar.CarDamageInformation.Engine.Damage = 1 - data.CarDamage.Engine;
            playerCar.CarDamageInformation.Transmission.Damage = 1 -data.CarDamage.Transmission;
            playerCar.TurboPressure = Math.Abs(data.TurboPressure) < 0.1 ? Pressure.Zero : Pressure.FromBar(data.TurboPressure);
            playerCar.OverallDownForce = Force.GetFromNewtons(data.Player.CurrentDownforce);

            playerCar.SpeedLimiterEngaged = data.PitLimiter == 1;

            FillDrsData(data, playerCar);
            FillBoostData(data.PushToPass, playerCar);
        }

        private static void FillDrsData(R3ESharedData data, CarInfo playerCar)
        {
            DrsSystem drsSystem = playerCar.DrsSystem;

            drsSystem.DrsActivationLeft = data.Drs.NumActivationsLeft < 100 ? data.Drs.NumActivationsLeft : -1;
            if (data.Drs.Equipped == 0)
            {
                drsSystem.DrsStatus = DrsStatus.NotEquipped;
                return;
            }

            if (data.Drs.Engaged == 1)
            {
                drsSystem.DrsStatus = DrsStatus.InUse;
                return;
            }

            if (data.Drs.Available == 1)
            {
                drsSystem.DrsStatus = DrsStatus.Available;
                return;
            }

            drsSystem.DrsStatus = DrsStatus.Equipped;
        }

        private static void FillBoostData(PushToPass pushToPass, CarInfo playerCar)
        {
            BoostSystem boostSystem = playerCar.BoostSystem;

            if (pushToPass.Available != 1)
            {
                boostSystem.BoostStatus = BoostStatus.UnAvailable;
                return;
            }

            boostSystem.ActivationsRemaining = pushToPass.AmountLeft;
            boostSystem.CooldownTimer = TimeSpan.FromSeconds(pushToPass.WaitTimeLeft);
            boostSystem.TimeRemaining = TimeSpan.FromSeconds(pushToPass.EngagedTimeLeft);

            if (pushToPass.Engaged == 1)
            {
                boostSystem.BoostStatus = BoostStatus.InUse;
                return;
            }

            boostSystem.BoostStatus = boostSystem.CooldownTimer == TimeSpan.Zero ? BoostStatus.Available : BoostStatus.Cooldown;
        }

        private void AddFlags(R3ESharedData data, SimulatorDataSet simData)
        {
            if (data.Flags.SectorYellow.Sector1 == 1)
            {
                simData.SessionInfo.ActiveFlags.Add(FlagKind.YellowSector1);
            }

            if (data.Flags.SectorYellow.Sector2 == 1)
            {
                simData.SessionInfo.ActiveFlags.Add(FlagKind.YellowSector2);
            }

            if (data.Flags.SectorYellow.Sector3 == 1)
            {
                simData.SessionInfo.ActiveFlags.Add(FlagKind.YellowSector3);
            }
        }

        private static void FillPlayerCarInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.EngineRpm = (int)(data.EngineRps / 0.104719755);
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
            simData.SessionInfo.TrackInfo.LayoutLength = Distance.FromMeters( data.LayoutLength);
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
            switch ((Constant.SessionLengthFormat)data.SessionLengthFormat)
            {
                case Constant.SessionLengthFormat.TimeBased:
                    simData.SessionInfo.SessionTimeRemaining = data.SessionTimeRemaining;
                    simData.SessionInfo.SessionLengthType = SessionLengthType.Time;
                    break;
                case Constant.SessionLengthFormat.LapBased:
                    simData.SessionInfo.SessionLengthType = SessionLengthType.Laps;
                    simData.SessionInfo.TotalNumberOfLaps = data.NumberOfLaps;
                    break;
                case Constant.SessionLengthFormat.TimeAndLapBased:
                    simData.SessionInfo.SessionTimeRemaining = data.SessionTimeRemaining;
                    simData.SessionInfo.SessionLengthType = SessionLengthType.TimeWitchExtraLap;
                    break;
                case Constant.SessionLengthFormat.Unavailable:
                default:
                    simData.SessionInfo.SessionLengthType = SessionLengthType.Na;
                    break;
            }
        }
    }
}
