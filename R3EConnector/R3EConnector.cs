using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using SecondMonitor.DataModel;
using SecondMonitor.PluginManager.GameConnector;
using SecondMonitor.R3EConnector.Data;
using SecondMonitor.DataModel.BasicProperties;
using System.Collections.Generic;
using static SecondMonitor.R3EConnector.Constant;

namespace SecondMonitor.R3EConnector
{

    public class R3EConnector : IGameConnector
    {

        private MemoryMappedFile sharedMemory;
        private readonly Queue<SimulatorDataSet> queue = new Queue<SimulatorDataSet>();
        private Thread daemonThread;
        private bool disconnect;
        private bool inSession;
        
        private R3RDatabase database;

        private static readonly string r3EExcecutable = "RRRE";
        private static readonly string SharedMemoryName = "$R3E";

        private TimeSpan timeInterval = TimeSpan.FromMilliseconds(100);

        public event EventHandler<DataEventArgs> DataLoaded;
        public event EventHandler<DataEventArgs> SessionStarted;
        public event EventHandler<EventArgs> ConnectedEvent;
        public event EventHandler<EventArgs> Disconnected;

        private DateTime lastTick;
        private TimeSpan sessionTime;
        private int lastSessionType;
        private int lastSessionPhase;
        private Double sessionStartR3RTime;
        private SecondMonitor.DataModel.Drivers.DriverInfo lastPlayer = new SecondMonitor.DataModel.Drivers.DriverInfo();





        public R3EConnector()
        {
            database = new R3RDatabase();
            database.Load();
            TickTime = 10;
            inSession = false;
            sessionTime = new TimeSpan(0);
            lastTick = DateTime.Now;
            sessionStartR3RTime = 0;
            lastSessionType = -2;
        }

        public bool IsConnected
        {
            get { return (sharedMemory != null); }
        }

        public int TickTime
        {
            get;
            set;
        }

        public static bool IsRrreRunning()
        {
            return Process.GetProcessesByName(r3EExcecutable).Length > 0;
        }

        public void AsynConnect()
        {
            Thread asyncConnectThread = new Thread(AsynConnector);
            asyncConnectThread.IsBackground = true;
            asyncConnectThread.Start();
        }

        public bool TryConnect()
        {
            return Connect();
        }

        private bool Connect()
        {
            if (!IsRrreRunning())
                return false;
            try
            {
                sharedMemory = MemoryMappedFile.OpenExisting(SharedMemoryName);
                RaiseConnectedEvent();
                StartDaemon();
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        private void AsynConnector()
        {
            while(!TryConnect())
            {
                Thread.Sleep(10);
            }
        }

        private void StartDaemon()
        {
            if (daemonThread != null)
                throw new InvalidOperationException("Daemon is already running");
            daemonThread = new Thread(DaemonMethod);
            daemonThread.IsBackground = true;
            daemonThread.Start();

            Thread queueProcessorThread = new Thread(QueueProcessor);
            queueProcessorThread.IsBackground = true;
            queueProcessorThread.Start();

        }

        private void DaemonMethod()
        {

            while (!disconnect)
            {

                Thread.Sleep(TickTime);
                R3ESharedData r3rData = Load();
                SimulatorDataSet data = FromR3EData(r3rData);
                if (CheckSessionStarted(r3rData))
                    RaiseSessionStartedEvent(data);
                DateTime tickTime = DateTime.Now;
                if (r3rData.GamePaused != 1 && r3rData.ControlType != (int) Constant.Control.Replay)
                {
                    //sessionTime = sessionTime.Add(tickTime.Subtract(lastTick));
                    sessionTime = TimeSpan.FromSeconds(r3rData.Player.GameSimulationTime - sessionStartR3RTime);
                }
                lastTick = tickTime;
                lock(queue)
                {
                    queue.Enqueue(data);
                }
                
            }
            sharedMemory = null;
            disconnect = false;
            RaiseDisconnectedEvent();
        }

        private void QueueProcessor()
        {
            while (true)
            {
                SimulatorDataSet set;
                while (queue.Count != 0)
                {
                    lock (queue)
                    {
                        set = queue.Dequeue();
                         
                    }
                    RaiseDataLoadedEvent(set);
                }
                Thread.Sleep(TickTime);
            }
            
        }



        private void Disconnect()
        {
            disconnect = true;            
            sharedMemory = null;
            RaiseDisconnectedEvent();
        }

        private R3ESharedData Load()
        {
            
                if (!IsConnected)
                    throw new InvalidOperationException("Not connected");
                R3ESharedData data;
                var _view = sharedMemory.CreateViewStream();
                BinaryReader _stream = new BinaryReader(_view);
                byte[] buffer = _stream.ReadBytes(Marshal.SizeOf(typeof(R3ESharedData)));
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                data = (R3ESharedData)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(R3ESharedData));
                handle.Free();
                return data;
            
        }


        private bool CheckSessionStarted(R3ESharedData r3rData)
        {
            if(r3rData.SessionType != lastSessionType)
            {
                lastSessionType = r3rData.SessionType;
                return true;
            }
            if(r3rData.SessionPhase != -1 && !inSession)
            {
                inSession = true;
                sessionStartR3RTime = r3rData.Player.GameSimulationTime;
                return true;
            }
            if(inSession && r3rData.SessionPhase == -1)
            {
                inSession = false;
            }
            if (lastSessionPhase >= 5 && r3rData.SessionPhase < 5 )
            {
                lastSessionPhase = r3rData.SessionPhase;
                return true;
            }
            lastSessionPhase = r3rData.SessionPhase;
            return false;
        }

        private void RaiseSessionStartedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            EventHandler<DataEventArgs> handler = SessionStarted;
            lastTick = DateTime.Now;
            sessionTime = new TimeSpan(0,0,1);
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void RaiseDataLoadedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            EventHandler<DataEventArgs> handler = DataLoaded;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void AddDriversData(SimulatorDataSet data, R3ESharedData r3rData)
        {
            if (r3rData.NumCars == -1)
                return;
            data.DriversInfo = new DataModel.Drivers.DriverInfo[r3rData.NumCars];
            String playerName = FromByteArray(r3rData.PlayerName);
            DataModel.Drivers.DriverInfo playersInfo = null;
            /*DataModel.Drivers.DriverInfo playerInfo = new DataModel.Drivers.DriverInfo();
            
            playerInfo.CompletedLaps = r3rData.CompletedLaps;
            playerInfo.CarName = System.Text.Encoding.UTF8.GetString(r3rData.VehicleInfo.Name).Replace("\0", "");
            playerInfo.InPits = r3rData.InPitlane == 1;
            playerInfo.IsPlayer = true;*/

            for (int i = 0; i<r3rData.NumCars; i++)
            {
                DriverData r3rDriverData =  r3rData.DriverData[i];
                DataModel.Drivers.DriverInfo driverInfo = new DataModel.Drivers.DriverInfo();
                driverInfo.DriverName = FromByteArray(r3rDriverData.DriverInfo.Name);
                driverInfo.CompletedLaps = r3rDriverData.CompletedLaps;
                driverInfo.CarName = "";//System.Text.Encoding.UTF8.GetString(r3rDriverData.DriverInfo.).Replace("\0", "");
                driverInfo.InPits = r3rDriverData.InPitlane == 1;
                driverInfo.IsPlayer = driverInfo.DriverName == playerName;
                driverInfo.Position = r3rDriverData.Place;
                driverInfo.Speed = r3rDriverData.CarSpeed;
                driverInfo.LapDistance = r3rDriverData.LapDistance;
                driverInfo.TotalDistance = r3rDriverData.CompletedLaps * r3rData.LayoutLength + r3rDriverData.LapDistance;
                ComputeDistanceToPlayer(lastPlayer, driverInfo, r3rData);
                driverInfo.CarName = database.GetCarName(r3rDriverData.DriverInfo.ModelId);
                driverInfo.FinishStatus = FromR3RStatus(r3rDriverData.FinishStatus);
                
                if (driverInfo.IsPlayer)
                {                    
                    playersInfo = driverInfo;
                    driverInfo.CurrentLapValid = r3rData.CurrentLapValid == 1;
                    lastPlayer = driverInfo;
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
                if (data.SessionInfo.SessionType == SessionInfo.SessionTypeEnum.Race && lastPlayer != null && lastPlayer.CompletedLaps!=0)
                {
                    driverInfo.IsBeingLappedByPlayer = driverInfo.TotalDistance < (lastPlayer.TotalDistance - r3rData.LayoutLength * 0.5);
                    driverInfo.IsLapingPlayer = lastPlayer.TotalDistance < (driverInfo.TotalDistance - r3rData.LayoutLength * 0.5);
                }

            }
            if (playersInfo != null)
            {
                data.PlayerInfo = playersInfo;
            }
        }

        private static SecondMonitor.DataModel.Drivers.DriverInfo.DriverFinishStatus FromR3RStatus(int finishStatus)
        {
            switch((FinishStatus)finishStatus)
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

        private static void ComputeDistanceToPlayer(DataModel.Drivers.DriverInfo player, DataModel.Drivers.DriverInfo driverInfo, R3ESharedData r3rData)
        {
            if (player == null)
                return;
            Single trackLength = r3rData.LayoutLength;
            Single playerLapDistance = player.LapDistance;

            Single distanceToPlayer = playerLapDistance - driverInfo.LapDistance;
            if (distanceToPlayer < -(trackLength / 2))
                distanceToPlayer = distanceToPlayer + trackLength;
            if (distanceToPlayer > (trackLength / 2))
                distanceToPlayer = distanceToPlayer - trackLength;
            driverInfo.DistanceToPlayer = distanceToPlayer;

        }

        //NEED EXTRACT WHEN SUPPORT FOR OTHER SIMS IS ADDED
        private SimulatorDataSet FromR3EData(R3ESharedData data)
        {
            SimulatorDataSet simData = new SimulatorDataSet("R3R");
            //SimulatorDataSet simData = new SimulatorDataSet("R3R");
            FillSessionInfo(data, simData);
            AddDriversData(simData, data);

            //PEDAL INFO
            simData.PedalInfo.ThrottlePedalPosition = data.ThrottlePedal;
            simData.PedalInfo.BrakePedalPosition = data.BrakePedal;
            simData.PedalInfo.ClutchPedalPosition = data.ClutchPedal;

            //WaterSystemInfo
            simData.PlayerInfo.CarInfo.WaterSystmeInfo.WaterTemperature = Temperature.FromCelsius(data.EngineWaterTemp);

            //OilSystemInfo
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilPressure = Pressure.FromKiloPascals(data.EngineOilPressure);
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(data.EngineOilTemp);

            //Brakes Info            
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature = Temperature.FromCelsius(data.BrakeTemp.FrontLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.BrakeTemperature = Temperature.FromCelsius(data.BrakeTemp.FrontRight);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.BrakeTemperature = Temperature.FromCelsius(data.BrakeTemp.RearLeft);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.BrakeTemperature = Temperature.FromCelsius(data.BrakeTemp.RearRight);


            //Tyre Pressure Info
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

            //Acceleration
            simData.PlayerInfo.CarInfo.Acceleration.XInMS = data.LocalAcceleration.X;
            simData.PlayerInfo.CarInfo.Acceleration.YInMS = data.LocalAcceleration.Y;
            simData.PlayerInfo.CarInfo.Acceleration.ZInMS = data.LocalAcceleration.Z;
            
            return simData;
        }

        private void FillSessionInfo(R3ESharedData data, SimulatorDataSet simData)
        {
            //Timing
            simData.SessionInfo.SessionTime = sessionTime; //TimeSpan.FromSeconds(data.Player.GameSimulationTime);
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
            }else if(data.NumberOfLaps != -1)
            {
                simData.SessionInfo.SessionLengthType = SessionInfo.SessionLengthTypeEnum.Laps;
                simData.SessionInfo.TotalNumberOfLaps = data.NumberOfLaps;
            }
        }

        private static string FromByteArray(byte[] buffer)
        {
            if(buffer[0]==(Char)0)
                return "";
            return System.Text.Encoding.UTF8.GetString(buffer).Split(new Char[] { (Char)0 }, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        private void RaiseConnectedEvent()
        {
            EventArgs args = new EventArgs();
            EventHandler<EventArgs> handler = ConnectedEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void RaiseDisconnectedEvent()
        {
            EventArgs args = new EventArgs();
            EventHandler<EventArgs> handler = Disconnected;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}
