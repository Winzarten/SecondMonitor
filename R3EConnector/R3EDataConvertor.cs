using SecondMonitor.DataModel;
using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.DataModel.Drivers;
using SecondMonitor.R3EConnector.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SecondMonitor.R3EConnector.Constant;

namespace SecondMonitor.R3EConnector
{
    internal class R3EDataConvertor
    {
        
        
        private R3RDatabase database;
        private DriverInfo _lastPlayer = new DriverInfo();
        private Dictionary<string, Single> lastLapTimes;
        private R3EConnector connector;

        internal R3EDataConvertor(R3EConnector connector)
        {
            this.connector = connector;
            database = new R3RDatabase();
            database.Load();
            lastLapTimes = new Dictionary<string, float>();
        }


        internal void AddDriversData(SimulatorDataSet data, R3ESharedData r3rData)
        {
            if (r3rData.NumCars == -1)
                return;
            data.DriversInfo = new DriverInfo[r3rData.NumCars];
            String playerName = FromByteArray(r3rData.PlayerName);
            DriverInfo playersInfo = null;
            /*DataModel.Drivers.DriverInfo playerInfo = new DataModel.Drivers.DriverInfo();
            
            playerInfo.CompletedLaps = r3rData.CompletedLaps;
            playerInfo.CarName = System.Text.Encoding.UTF8.GetString(r3rData.VehicleInfo.Name).Replace("\0", "");
            playerInfo.InPits = r3rData.InPitlane == 1;
            playerInfo.IsPlayer = true;*/

            for (int i = 0; i < r3rData.NumCars; i++)
            {
                DriverData r3rDriverData = r3rData.DriverData[i];
                DriverInfo driverInfo = new DriverInfo();
                driverInfo.DriverName = FromByteArray(r3rDriverData.DriverInfo.Name);
                driverInfo.CompletedLaps = r3rDriverData.CompletedLaps;
                driverInfo.CarName = "";//System.Text.Encoding.UTF8.GetString(r3rDriverData.DriverInfo.).Replace("\0", "");
                driverInfo.InPits = r3rDriverData.InPitlane == 1;
                driverInfo.IsPlayer = driverInfo.DriverName == playerName;
                driverInfo.Position = r3rDriverData.Place;
                driverInfo.Speed = Velocity.FromMs(r3rDriverData.CarSpeed);
                driverInfo.LapDistance = r3rDriverData.LapDistance;
                driverInfo.TotalDistance = r3rDriverData.CompletedLaps * r3rData.LayoutLength + r3rDriverData.LapDistance;                
                driverInfo.CarName = database.GetCarName(r3rDriverData.DriverInfo.ModelId);
                driverInfo.FinishStatus = FromR3RStatus(r3rDriverData.FinishStatus);
                ComputeDistanceToPlayer(_lastPlayer, driverInfo, r3rData);

                if (driverInfo.IsPlayer)
                {
                    playersInfo = driverInfo;
                    driverInfo.CurrentLapValid = r3rData.CurrentLapValid == 1;
                    _lastPlayer = driverInfo;
                }
                else
                    driverInfo.CurrentLapValid = r3rDriverData.CurrentLapValid == 1;
                driverInfo.CarInfo.WheelsInfo.FrontLeft.TyreType = ((TireSubtype)r3rDriverData.TireSubtypeFront).ToString();
                driverInfo.CarInfo.WheelsInfo.FrontRight.TyreTypeFilled = ((TireSubtype)r3rDriverData.TireSubtypeFront) != TireSubtype.Unavailable;
                driverInfo.CarInfo.WheelsInfo.FrontRight.TyreType = driverInfo.CarInfo.WheelsInfo.FrontLeft.TyreType;
                driverInfo.CarInfo.WheelsInfo.FrontRight.TyreTypeFilled = driverInfo.CarInfo.WheelsInfo.FrontLeft.TyreTypeFilled;

                driverInfo.CarInfo.WheelsInfo.RearLeft.TyreTypeFilled = ((TireSubtype)r3rDriverData.TireSubtypeRear) != TireSubtype.Unavailable;
                driverInfo.CarInfo.WheelsInfo.RearLeft.TyreType = ((TireSubtype)r3rDriverData.TireSubtypeRear).ToString();
                driverInfo.CarInfo.WheelsInfo.RearRight.TyreType = driverInfo.CarInfo.WheelsInfo.RearLeft.TyreType;
                driverInfo.CarInfo.WheelsInfo.RearRight.TyreTypeFilled = driverInfo.CarInfo.WheelsInfo.RearLeft.TyreTypeFilled;
                data.DriversInfo[i] = driverInfo;
                if (driverInfo.Position == 1)
                {
                    data.SessionInfo.LeaderCurrentLap = driverInfo.CompletedLaps + 1;
                    data.LeaderInfo = driverInfo;
                }
                if (data.SessionInfo.SessionType == SessionInfo.SessionTypeEnum.Race && _lastPlayer != null && _lastPlayer.CompletedLaps != 0)
                {
                    driverInfo.IsBeingLappedByPlayer = driverInfo.TotalDistance < (_lastPlayer.TotalDistance - r3rData.LayoutLength * 0.5);
                    driverInfo.IsLapingPlayer = _lastPlayer.TotalDistance < (driverInfo.TotalDistance - r3rData.LayoutLength * 0.5);
                }
                FillTimingInfor(driverInfo, r3rDriverData, r3rData);
                if (driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Finished)
                    driverInfo.CompletedLaps++;
                connector.StoreLastTickInfo(driverInfo);
            }
            if (playersInfo != null)
            {
                data.PlayerInfo = playersInfo;
            }
        }

        internal void FillTimingInfor(DriverInfo driverInfo, DriverData r3eDriverData, R3ESharedData r3rData)
        {
            if (driverInfo.IsPlayer)
            {
                driverInfo.Timing.LastLapTime = r3rData.LapTimePreviousSelf;
                return;
            }
            else
                driverInfo.Timing.LastLapTime = r3eDriverData.SectorTimePreviousSelf.Sector3;
        }

        internal static DriverInfo.DriverFinishStatus FromR3RStatus(int finishStatus)
        {
            switch ((FinishStatus)finishStatus)
            {
                case FinishStatus.Unavailable:
                    return DataModel.Drivers.DriverInfo.DriverFinishStatus.NA;
                case FinishStatus.DNF:
                    return DataModel.Drivers.DriverInfo.DriverFinishStatus.DNF;
                case FinishStatus.DNQ:
                    return DataModel.Drivers.DriverInfo.DriverFinishStatus.DNQ;
                case FinishStatus.DNS:
                    return DataModel.Drivers.DriverInfo.DriverFinishStatus.DNS;
                case FinishStatus.DQ:
                    return DataModel.Drivers.DriverInfo.DriverFinishStatus.DQ;
                case FinishStatus.Finished:
                    return DataModel.Drivers.DriverInfo.DriverFinishStatus.Finished;
                case FinishStatus.None:
                    return DataModel.Drivers.DriverInfo.DriverFinishStatus.None;
            }
            return DataModel.Drivers.DriverInfo.DriverFinishStatus.NA;

        }

        internal static void ComputeDistanceToPlayer(DriverInfo player, DriverInfo driverInfo, R3ESharedData r3rData)
        {
            if (player == null)
                return;
            if(driverInfo.FinishStatus==DriverInfo.DriverFinishStatus.DQ || driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.DNF || 
                driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.DNQ || driverInfo.FinishStatus == DriverInfo.DriverFinishStatus.DNS)
            {
                driverInfo.DistanceToPlayer = Single.MaxValue;
                return;
            }
            Single trackLength = r3rData.LayoutLength;
            Single playerLapDistance = player.LapDistance;

            Single distanceToPlayer = playerLapDistance - driverInfo.LapDistance;
            if (distanceToPlayer < -(trackLength / 2))
                distanceToPlayer = distanceToPlayer + trackLength;
            if (distanceToPlayer > (trackLength / 2))
                distanceToPlayer = distanceToPlayer - trackLength;
            driverInfo.DistanceToPlayer = distanceToPlayer;

        }


        internal SimulatorDataSet FromR3EData(R3ESharedData data)
        {
            SimulatorDataSet simData = new SimulatorDataSet("R3R");
            simData.SimulatorSourceInfo.HasLapTimeInformation = true;
            //SimulatorDataSet simData = new SimulatorDataSet("R3R");
            FillSessionInfo(data, simData);
            AddDriversData(simData, data);

            //PEDAL INFO
            AddPedalInfo(data, simData);

            //WaterSystemInfo
            AddWaterSystemInfo(data, simData);

            //OilSystemInfo
            AddOilSystemInfo(data, simData);

            //Brakes Info
            AddBrakesInfo(data, simData);


            //Tyre Pressure Info
            AddTyresInfo(data, simData);

            //Acceleration
            AddAcceleration(data, simData);

            return simData;
        }

        private static void AddAcceleration(R3ESharedData data, SimulatorDataSet simData)
        {
            simData.PlayerInfo.CarInfo.Acceleration.XInMS = data.LocalAcceleration.X;
            simData.PlayerInfo.CarInfo.Acceleration.YInMS = data.LocalAcceleration.Y;
            simData.PlayerInfo.CarInfo.Acceleration.ZInMS = data.LocalAcceleration.Z;
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

            //Front Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.LeftTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontLeft_Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RightTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontLeft_Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.CenterTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontLeft_Center);


            //Front Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.LeftTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontRight_Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RightTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontRight_Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.CenterTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontRight_Center);


            //Rear Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.LeftTyreTemp = Temperature.FromCelsius(data.TireTemp.RearLeft_Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RightTyreTemp = Temperature.FromCelsius(data.TireTemp.RearLeft_Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.CenterTyreTemp = Temperature.FromCelsius(data.TireTemp.RearLeft_Center);

            //Rear Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.LeftTyreTemp = Temperature.FromCelsius(data.TireTemp.RearRight_Left);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RightTyreTemp = Temperature.FromCelsius(data.TireTemp.RearRight_Right);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.CenterTyreTemp = Temperature.FromCelsius(data.TireTemp.RearRight_Center);

            //Fuel System
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

        internal void FillSessionInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            //Timing
            simData.SessionInfo.SessionTime = connector.SessionTime; //TimeSpan.FromSeconds(data.Player.GameSimulationTime);
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
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.NA;
                    break;
                case Constant.Session.Warmup:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.WarmUp;
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

            }
            simData.SessionInfo.TrackName = FromByteArray(data.TrackName);
            simData.SessionInfo.TrackLayoutName = FromByteArray(data.LayoutName);
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

        private static string FromByteArray(byte[] buffer)
        {
            if (buffer[0] == (Char)0)
                return "";
            return System.Text.Encoding.UTF8.GetString(buffer).Split(new Char[] { (Char)0 }, StringSplitOptions.RemoveEmptyEntries)[0];
        }
    }
}
