using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using SecondMonitor.DataModel;
using SecondMonitor.PluginManager.GameConnector;
using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.PCarsConnector.enums;
using System.Collections.Generic;
using SecondMonitor.DataModel.Drivers;

namespace SecondMonitor.PCarsConnector
{

    public class PCarsConnector : IGameConnector
    {

        public class NameNotFilledException : Exception
        {       
            public NameNotFilledException(string msg) : base(msg)
            {
            }
        }

        private MemoryMappedFile sharedMemory;
        private static int sharedmemorysize;
        private static byte[] sharedMemoryReadBuffer;
        private static bool isSharedMemoryInitialised = false;
        private static GCHandle handle;
        private Thread daemonThread;
        private bool disconnect;

        private static readonly string r3EExcecutable = "pCARS64";
        private static readonly string SharedMemoryName = "$pcars$";

        private TimeSpan timeInterval = TimeSpan.FromMilliseconds(100);

        public event EventHandler<DataEventArgs> DataLoaded;
        public event EventHandler<EventArgs> ConnectedEvent;
        public event EventHandler<EventArgs> Disconnected;
        public event EventHandler<DataEventArgs> SessionStarted;

        private DateTime lastTick;
        private TimeSpan sessionTime;
        private double nextSpeedComputation;
        private SimulatorDataSet lastSpeedComputationSet;

        private readonly TimeSpan pitTimeDelay = TimeSpan.FromMilliseconds(2000);
        private readonly int pitRaceTimeCheckDelay = 20000;

        private uint lastSessionState = 0;
        private SimulatorDataSet previousSet = new SimulatorDataSet("PCARS");
        private Dictionary<string, TimeSpan> pitTriggerTimes;
        private Dictionary<string, DriverInfo> previousTickInfo = new Dictionary<string, DriverInfo>();



        public PCarsConnector()
        {
            TickTime = 10;
            ResetConnector();
        }

        private void ResetConnector()
        {
            sessionTime = new TimeSpan(0);
            disconnect = false;
            pitTriggerTimes = new Dictionary<string, TimeSpan>();
        }

        public bool IsConnected
        {
            get { return (sharedMemory != null && isSharedMemoryInitialised); }
        }

        public int TickTime
        {
            get;
            set;
        }

        public static bool IsPCarsRunning()
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
            if (!IsPCarsRunning())
                return false;
            try
            {
                sharedMemory = MemoryMappedFile.OpenExisting(SharedMemoryName);
                sharedmemorysize = Marshal.SizeOf(typeof(pCarsAPIStruct));
                sharedMemoryReadBuffer = new byte[sharedmemorysize];
                isSharedMemoryInitialised = true;
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
            while (!TryConnect())
            {
                Thread.Sleep(10);
            }
        }

        private void StartDaemon()
        {
            if (daemonThread != null && daemonThread.IsAlive)
                throw new InvalidOperationException("Daemon is already running");
            ResetConnector();
            daemonThread = new Thread(DaemonMethod);
            daemonThread.IsBackground = true;
            daemonThread.Start();
        }

        private void DaemonMethod()
        {

            while (!disconnect)
            {

                Thread.Sleep(TickTime);
                pCarsAPIStruct data = Load();
                try
                {
                    DateTime tickTime = DateTime.Now;
                    TimeSpan lastTickDuration = tickTime.Subtract(lastTick);
                    SimulatorDataSet simData= FromPCARSData(data, lastTickDuration);

                    if (lastSessionState != data.mSessionState)
                    {
                        pitTriggerTimes.Clear();
                        previousTickInfo.Clear();
                        nextSpeedComputation = 0;
                        RaiseSessionStartedEvent(simData);
                    }

                    lastSessionState = data.mSessionState;
                    RaiseDataLoadedEvent(simData);
                    if (!IsPCarsRunning())
                        disconnect = true;
                    lastTick = tickTime;
                    previousSet = simData;
                }
                catch (NameNotFilledException ex)
                {
                    //Ignore, names are sometimes not set in the shared memory
                }
            }

            sharedMemory = null;
            RaiseDisconnectedEvent();
        }



        private void Disconnect()
        {
            disconnect = true;

            sharedMemory = null;
            RaiseDisconnectedEvent();
        }

        private pCarsAPIStruct Load()
        {
            lock (sharedMemory)
            {
                pCarsAPIStruct _pcarsapistruct = new pCarsAPIStruct();
                if (!IsConnected)
                    throw new InvalidOperationException("Not connected");

                using (var sharedMemoryStreamView = sharedMemory.CreateViewStream())
                {
                    BinaryReader _SharedMemoryStream = new BinaryReader(sharedMemoryStreamView);
                    sharedMemoryReadBuffer = _SharedMemoryStream.ReadBytes(sharedmemorysize);
                    handle = GCHandle.Alloc(sharedMemoryReadBuffer, GCHandleType.Pinned);
                    _pcarsapistruct = (pCarsAPIStruct)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(pCarsAPIStruct));
                    handle.Free();
                }

                return _pcarsapistruct;
            }
        }


        private void RaiseDataLoadedEvent(SimulatorDataSet simData)
        {
                       
            DataEventArgs args = new DataEventArgs(simData);
            DataLoaded?.Invoke(this, args);
        }

        private void RaiseSessionStartedEvent(SimulatorDataSet data)
        {            
            DataEventArgs args = new DataEventArgs(data);
            EventHandler<DataEventArgs> handler = SessionStarted;
            lastTick = DateTime.Now;
            sessionTime = new TimeSpan(0, 0, 1);
            handler?.Invoke(this, args);
        }

        private void AddDriversData(pCarsAPIStruct pCarsData, SimulatorDataSet data, TimeSpan lastTickDuration)
        {
            if (pCarsData.mNumParticipants == -1)
                return;
            TrackDetails trackDetails =
                TrackDetails.GetTrackDetails(data.SessionInfo.TrackName, data.SessionInfo.TrackLayoutName);
            data.DriversInfo = new DataModel.Drivers.DriverInfo[pCarsData.mNumParticipants];
            //String playerName = "Darkman";// FromByteArray(r3rData.PlayerName);
            DataModel.Drivers.DriverInfo playersInfo = null;
            /*DataModel.Drivers.DriverInfo playerInfo = new DataModel.Drivers.DriverInfo();
            
            playerInfo.CompletedLaps = r3rData.CompletedLaps;
            playerInfo.CarName = System.Text.Encoding.UTF8.GetString(r3rData.VehicleInfo.Name).Replace("\0", "");
            playerInfo.InPits = r3rData.InPitlane == 1;
            playerInfo.IsPlayer = true;*/
            bool computeSpeed = data.SessionInfo.SessionTime.TotalSeconds > nextSpeedComputation;
            for (int i = 0; i < pCarsData.mNumParticipants; i++)
            {
                pCarsAPIParticipantStruct r3rDriverData = pCarsData.mParticipantData[i];
                DataModel.Drivers.DriverInfo driverInfo = new DataModel.Drivers.DriverInfo();
                driverInfo.DriverName = r3rDriverData.mName;
                driverInfo.CompletedLaps = (int)r3rDriverData.mLapsCompleted;
                driverInfo.CarName = "";//System.Text.Encoding.UTF8.GetString(r3rDriverData.DriverInfo.).Replace("\0", "");
                driverInfo.InPits = false;// r3rDriverData.InPitlane == 1;
                driverInfo.IsPlayer = i == pCarsData.mViewedParticipantIndex;
                driverInfo.Position =  (int)r3rDriverData.mRacePosition;
                driverInfo.LapDistance = r3rDriverData.mCurrentLapDistance;
                driverInfo.CarName = "NA";// database.GetCarName(r3rDriverData.DriverInfo.ModelId);
                driverInfo.CurrentLapValid = true;
                driverInfo.WorldPostion = new Point3D(r3rDriverData.mWorldPosition[0], r3rDriverData.mWorldPosition[1], r3rDriverData.mWorldPosition[2]);
                if (driverInfo.IsPlayer)
                {
                    playersInfo = driverInfo;
                    driverInfo.CurrentLapValid = !pCarsData.mLapInvalidated;
                }
                else
                    driverInfo.CurrentLapValid = true;
                data.DriversInfo[i] = driverInfo;
                if (String.IsNullOrEmpty(driverInfo.DriverName))
                    throw new NameNotFilledException("Name not filled for driver with index "+i);
                if(!computeSpeed && previousTickInfo.ContainsKey(driverInfo.DriverName))
                {
                    driverInfo.Speed = previousTickInfo[driverInfo.DriverName].Speed;
                }
                if(previousTickInfo.ContainsKey(driverInfo.DriverName) && computeSpeed)
                {
                    Point3D currentWorldPosition = driverInfo.WorldPostion;
                    Point3D previousWorldPosition = previousTickInfo[driverInfo.DriverName].WorldPostion;
                    double duration = data.SessionInfo.SessionTime.Subtract(lastSpeedComputationSet.SessionInfo.SessionTime).TotalSeconds;
                    //double speed = lastTickDuration.TotalMilliseconds;
                    double speed = Math.Sqrt(Math.Pow(currentWorldPosition.X - previousWorldPosition.X, 2) + Math.Pow(currentWorldPosition.Y - previousWorldPosition.Y, 2)+Math.Pow(currentWorldPosition.Z - previousWorldPosition.Z, 2))/ duration;
                    if(speed < 200)
                        driverInfo.Speed = Velocity.FromMs(speed);
                }
                if(computeSpeed)
                {
                    lastSpeedComputationSet = data;
                    previousTickInfo[driverInfo.DriverName] = driverInfo;
                }
            }
            if (computeSpeed)
                nextSpeedComputation += 0.5;
            if (playersInfo != null)
            {
                ComputeDistanceToPlayer(playersInfo, data);
                data.PlayerInfo = playersInfo;
            }
            AddPitsInfo(data);
            
        }

        private static void ComputeDistanceToPlayer(DataModel.Drivers.DriverInfo player, SimulatorDataSet data)
        {
            Single trackLength = data.SessionInfo.LayoutLength;
            Single playerLapDistance = player.LapDistance;
            foreach (var driverInfo in data.DriversInfo)
            {
                Single distanceToPlayer = playerLapDistance - driverInfo.LapDistance;
                if (distanceToPlayer < -(trackLength / 2))
                    distanceToPlayer = distanceToPlayer + trackLength;
                if (distanceToPlayer > (trackLength / 2))
                    distanceToPlayer = distanceToPlayer - trackLength;
                driverInfo.DistanceToPlayer = distanceToPlayer;
            }
        }

        private void FillSessionInfo(pCarsAPIStruct pCarsData, SimulatorDataSet simData, TimeSpan lastTickDuration)
        {
            
            if (pCarsData.mGameState == 2)
            {
                sessionTime = sessionTime.Add(lastTickDuration);
            }
            if (pCarsData.mSessionState == (int)eSessionState.SESSION_INVALID || 
                (pCarsData.mSessionState == (int)eSessionState.SESSION_RACE && pCarsData.mRaceState == 1))
                sessionTime = new TimeSpan(0);
            simData.SessionInfo.SessionTime = sessionTime;
            

            simData.SessionInfo.WeatherInfo.airTemperature = Temperature.FromCelsius(pCarsData.mAmbientTemperature);
            simData.SessionInfo.WeatherInfo.trackTemperature = Temperature.FromCelsius(pCarsData.mTrackTemperature);
            simData.SessionInfo.WeatherInfo.rainIntensity = (int)(pCarsData.mRainDensity * 100);
            simData.SessionInfo.LayoutLength = pCarsData.mTrackLength;
            simData.SessionInfo.IsActive = true; // (eRaceState)pCarsData.mRaceState == eRaceState.RACESTATE_RACING 
                //|| (eRaceState)pCarsData.mRaceState == eRaceState.RACESTATE_FINISHED;
            switch ((eSessionState)pCarsData.mSessionState)
            {
                case eSessionState.SESSION_PRACTICE:
                case eSessionState.SESSION_TEST:
                case eSessionState.SESSION_TIME_ATTACK:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.Practice;
                    break;
                case eSessionState.SESSION_QUALIFY:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.Qualification;
                    break;
                case eSessionState.SESSION_RACE:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.Race;
                    break;
                case eSessionState.SESSION_INVALID:
                    simData.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.NA;
                    break;
                
            }
            switch ((eRaceState)pCarsData.mRaceState)
            {
                case eRaceState.RACESTATE_NOT_STARTED:                
                    simData.SessionInfo.SessionPhase = SessionInfo.SessionPhaseEnum.Countdown;
                    break;
                case eRaceState.RACESTATE_RACING:
                    simData.SessionInfo.SessionPhase = SessionInfo.SessionPhaseEnum.Green;
                    break;
                case eRaceState.RACESTATE_RETIRED:
                case eRaceState.RACESTATE_DNF:
                case eRaceState.RACESTATE_DISQUALIFIED:
                case eRaceState.RACESTATE_FINISHED:
                    simData.SessionInfo.SessionPhase = SessionInfo.SessionPhaseEnum.Checkered;
                    break;

            }
            if (simData.SessionInfo.SessionPhase == SessionInfo.SessionPhaseEnum.Countdown && simData.SessionInfo.SessionType != SessionInfo.SessionTypeEnum.Race)
                simData.SessionInfo.SessionPhase = SessionInfo.SessionPhaseEnum.Green;
            simData.SessionInfo.TrackName = pCarsData.mTrackLocation;
            simData.SessionInfo.TrackLayoutName = pCarsData.mTrackVariation;

            if (pCarsData.mEventTimeRemaining != -1)
            {
                simData.SessionInfo.SessionLengthType = SessionInfo.SessionLengthTypeEnum.Time;
                simData.SessionInfo.SessionTimeRemaining = pCarsData.mEventTimeRemaining / 1000;
            }
            else if (pCarsData.mLapsInEvent != 0)
            {
                simData.SessionInfo.SessionLengthType = SessionInfo.SessionLengthTypeEnum.Laps;
                simData.SessionInfo.TotalNumberOfLaps = (int)pCarsData.mLapsInEvent;
            }

        }

        private bool ShouldCheckPits(SimulatorDataSet dataSet)
        {            
            if (dataSet.SessionInfo.SessionType != SessionInfo.SessionTypeEnum.Race)
               return dataSet.SessionInfo.IsActive;
            return dataSet.SessionInfo.SessionTime.TotalMilliseconds > pitRaceTimeCheckDelay;
        }

        private void AddPitsInfo(SimulatorDataSet dataSet)
        {
            if (!ShouldCheckPits(dataSet))
                return;
            foreach (var driverInfo in dataSet.DriversInfo)
            {
                driverInfo.InPits = false;
                if (driverInfo.LapDistance != 0 && pitTriggerTimes.ContainsKey(driverInfo.DriverName))
                    pitTriggerTimes.Remove(driverInfo.DriverName);
                if (driverInfo.LapDistance != 0)
                    continue;
                if (driverInfo.LapDistance == 0)
                {
                    if (!pitTriggerTimes.ContainsKey(driverInfo.DriverName))
                    {
                        pitTriggerTimes[driverInfo.DriverName] = dataSet.SessionInfo.SessionTime.Add(pitTimeDelay);
                        continue;
                    }
                    if (dataSet.SessionInfo.SessionTime > pitTriggerTimes[driverInfo.DriverName])
                        driverInfo.InPits = true;
                }
            }
        }

        //NEED EXTRACT WHEN SUPPORT FOR OTHER SIMS IS ADDED
        private SimulatorDataSet FromPCARSData(pCarsAPIStruct data, TimeSpan lastTickDuration)
        {
            SimulatorDataSet simData = new SimulatorDataSet("PCars");
            //PEDAL INFO
            simData.PedalInfo.ThrottlePedalPosition = data.mThrottle;
            simData.PedalInfo.BrakePedalPosition = data.mBrake;
            simData.PedalInfo.ClutchPedalPosition = data.mClutch;

            FillSessionInfo(data, simData, lastTickDuration);
            AddDriversData(data, simData, lastTickDuration);

            //WaterSystemInfo
            simData.PlayerInfo.CarInfo.WaterSystmeInfo.WaterTemperature = Temperature.FromCelsius(data.mWaterTempCelsius);

            //OilSystemInfo
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilPressure = Pressure.FromKiloPascals(data.mOilPressureKPa);
            simData.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(data.mOilTempCelsius);

            //Brakes Info            
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature = Temperature.FromCelsius(data.mBrakeTempCelsius[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.BrakeTemperature = Temperature.FromCelsius(data.mBrakeTempCelsius[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.BrakeTemperature = Temperature.FromCelsius(data.mBrakeTempCelsius[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.BrakeTemperature = Temperature.FromCelsius(data.mBrakeTempCelsius[3]);

            //Tyre Pressure Info
            /*simData.PlayerCarInfo.WheelsInfo.FrontLeft.TyrePressure = Pressure.FromKiloPascals(data.ty TirePressure.FrontLeft);
            simData.PlayerCarInfo.WheelsInfo.FrontRight.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.FrontRight);
            simData.PlayerCarInfo.WheelsInfo.RearLeft.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.RearLeft);
            simData.PlayerCarInfo.WheelsInfo.RearRight.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.RearRight);*/

            //Front Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.LeftTyreTemp = Temperature.FromCelsius(data.mTyreTemp[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.RightTyreTemp = Temperature.FromCelsius(data.mTyreTemp[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.CenterTyreTemp = Temperature.FromCelsius(data.mTyreTemp[0]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.TyreWear = data.mTyreWear[0];

            //Front Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.LeftTyreTemp = Temperature.FromCelsius(data.mTyreTemp[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.RightTyreTemp = Temperature.FromCelsius(data.mTyreTemp[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.CenterTyreTemp = Temperature.FromCelsius(data.mTyreTemp[1]);
            simData.PlayerInfo.CarInfo.WheelsInfo.FrontRight.TyreWear = data.mTyreWear[1];

            //Rear Left Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.LeftTyreTemp = Temperature.FromCelsius(data.mTyreTemp[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.RightTyreTemp = Temperature.FromCelsius(data.mTyreTemp[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.CenterTyreTemp = Temperature.FromCelsius(data.mTyreTemp[2]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearLeft.TyreWear = data.mTyreWear[2];

            //Rear Right Tyre Temps
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.LeftTyreTemp = Temperature.FromCelsius(data.mTyreTemp[3]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.RightTyreTemp = Temperature.FromCelsius(data.mTyreTemp[3]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.CenterTyreTemp = Temperature.FromCelsius(data.mTyreTemp[3]);
            simData.PlayerInfo.CarInfo.WheelsInfo.RearRight.TyreWear = data.mTyreWear[3];

            //Fuel
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(data.mFuelCapacity);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining = Volume.FromLiters(data.mFuelCapacity * data.mFuelLevel);
            simData.PlayerInfo.CarInfo.FuelSystemInfo.FuelPressure = Pressure.FromKiloPascals(data.mFuelPressureKPa);

            //Acceleration
            simData.PlayerInfo.CarInfo.Acceleration.XInMS = data.mLocalAcceleration[0];
            simData.PlayerInfo.CarInfo.Acceleration.YInMS = data.mLocalAcceleration[1];
            simData.PlayerInfo.CarInfo.Acceleration.ZInMS = data.mLocalAcceleration[2];

            
            return simData;
        }

        private void RaiseConnectedEvent()
        {
            EventArgs args = new EventArgs();
            ConnectedEvent?.Invoke(this, args);
        }

        private void RaiseDisconnectedEvent()
        {
            EventArgs args = new EventArgs();
            Disconnected?.Invoke(this, args);
        }
    }
}
