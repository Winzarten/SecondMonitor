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

namespace SecondMonitor.PCarsConnector
{

    public class PCarsConnector : IGameConnector
    {

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

        private DateTime lastTick;
        private TimeSpan sessionTime;

        

        public PCarsConnector()
        {
            TickTime = 10;
            sessionTime = new TimeSpan(0);
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
        }

        private void DaemonMethod()
        {
            
            while (!disconnect )
            {

               Thread.Sleep(TickTime);
                pCarsAPIStruct data = Load();
               RaiseDataLoadedEvent(data);
            }
            
            sharedMemory = null;
            disconnect = false;
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


        private void RaiseDataLoadedEvent(pCarsAPIStruct data)
        {
            SimulatorDataSet simData = FromR3EData(data);
            DateTime tickTime = DateTime.Now;
            if (data.mGameState == 2)
            {
                sessionTime = sessionTime.Add(tickTime.Subtract(lastTick));
            }
            if (data.mSessionState == 0 || data.mRaceState == 1)
                sessionTime = new TimeSpan(0);
            simData.SessionInfo.SessionTime = sessionTime;
            lastTick = tickTime;
            DataEventArgs args = new DataEventArgs(simData);
            EventHandler<DataEventArgs> handler = DataLoaded;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        //NEED EXTRACT WHEN SUPPORT FOR OTHER SIMS IS ADDED
        private static SimulatorDataSet FromR3EData(pCarsAPIStruct data)
        {
            SimulatorDataSet simData = new SimulatorDataSet();            
            //PEDAL INFO
            simData.PedalInfo.ThrottlePedalPosition = data.mThrottle;
            simData.PedalInfo.BrakePedalPosition = data.mBrake;

            //WaterSystemInfo
            simData.PlayerCarInfo.WaterSystmeInfo.WaterTemperature = Temperature.FromCelsius(data.mWaterTempCelsius);

            //OilSystemInfo
            simData.PlayerCarInfo.OilSystemInfo.OilPressure = Pressure.FromKiloPascals(data.mOilPressureKPa);
            simData.PlayerCarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(data.mOilTempCelsius);

            //Brakes Info            
            simData.PlayerCarInfo.WheelsInfo.FrontLeft.BrakeTemperature = Temperature.FromCelsius(data.mBrakeTempCelsius[0]);
            simData.PlayerCarInfo.WheelsInfo.FrontRight.BrakeTemperature = Temperature.FromCelsius(data.mBrakeTempCelsius[1]);
            simData.PlayerCarInfo.WheelsInfo.RearLeft.BrakeTemperature = Temperature.FromCelsius(data.mBrakeTempCelsius[2]);
            simData.PlayerCarInfo.WheelsInfo.RearRight.BrakeTemperature = Temperature.FromCelsius(data.mBrakeTempCelsius[3]);

            //Tyre Pressure Info
            /*simData.PlayerCarInfo.WheelsInfo.FrontLeft.TyrePressure = Pressure.FromKiloPascals(data.ty TirePressure.FrontLeft);
            simData.PlayerCarInfo.WheelsInfo.FrontRight.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.FrontRight);
            simData.PlayerCarInfo.WheelsInfo.RearLeft.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.RearLeft);
            simData.PlayerCarInfo.WheelsInfo.RearRight.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.RearRight);*/

            //Front Left Tyre Temps
            simData.PlayerCarInfo.WheelsInfo.FrontLeft.LeftTyreTemp = Temperature.FromCelsius(data.mTyreTemp[0]);
            simData.PlayerCarInfo.WheelsInfo.FrontLeft.RightTyreTemp = Temperature.FromCelsius(data.mTyreTemp[0]);
            simData.PlayerCarInfo.WheelsInfo.FrontLeft.CenterTyreTemp = Temperature.FromCelsius(data.mTyreTemp[0]);
            simData.PlayerCarInfo.WheelsInfo.FrontLeft.TyreWear = data.mTyreWear[0];

            //Front Right Tyre Temps
            simData.PlayerCarInfo.WheelsInfo.FrontRight.LeftTyreTemp = Temperature.FromCelsius(data.mTyreTemp[1]);
            simData.PlayerCarInfo.WheelsInfo.FrontRight.RightTyreTemp = Temperature.FromCelsius(data.mTyreTemp[1]);
            simData.PlayerCarInfo.WheelsInfo.FrontRight.CenterTyreTemp = Temperature.FromCelsius(data.mTyreTemp[1]);
            simData.PlayerCarInfo.WheelsInfo.FrontRight.TyreWear = data.mTyreWear[1];

            //Rear Left Tyre Temps
            simData.PlayerCarInfo.WheelsInfo.RearLeft.LeftTyreTemp = Temperature.FromCelsius(data.mTyreTemp[2]);
            simData.PlayerCarInfo.WheelsInfo.RearLeft.RightTyreTemp = Temperature.FromCelsius(data.mTyreTemp[2]);
            simData.PlayerCarInfo.WheelsInfo.RearLeft.CenterTyreTemp = Temperature.FromCelsius(data.mTyreTemp[2]);
            simData.PlayerCarInfo.WheelsInfo.RearLeft.TyreWear = data.mTyreWear[2];

            //Rear Right Tyre Temps
            simData.PlayerCarInfo.WheelsInfo.RearRight.LeftTyreTemp = Temperature.FromCelsius(data.mTyreTemp[3]);
            simData.PlayerCarInfo.WheelsInfo.RearRight.RightTyreTemp = Temperature.FromCelsius(data.mTyreTemp[3]);
            simData.PlayerCarInfo.WheelsInfo.RearRight.CenterTyreTemp = Temperature.FromCelsius(data.mTyreTemp[3]);
            simData.PlayerCarInfo.WheelsInfo.RearRight.TyreWear = data.mTyreWear[3];

            //Fuel
            simData.PlayerCarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(data.mFuelCapacity);
            simData.PlayerCarInfo.FuelSystemInfo.FuelRemaining = Volume.FromLiters(data.mFuelCapacity * data.mFuelLevel);
            simData.PlayerCarInfo.FuelSystemInfo.FuelPressure = Pressure.FromKiloPascals(data.mFuelPressureKPa);

            //Acceleration
            simData.PlayerCarInfo.Acceleration.XInMS = data.mLocalAcceleration[0];
            simData.PlayerCarInfo.Acceleration.YInMS = data.mLocalAcceleration[1];
            simData.PlayerCarInfo.Acceleration.ZInMS = data.mLocalAcceleration[2];

            return simData;
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
