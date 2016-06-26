using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using SecondMonitor.Core.R3EConnector.Data;
using System.Threading;
using SecondMonitor.DataModel;
using SecondMonitor.PluginManager.GameConnector;

namespace SecondMonitor.Core.R3EConnector
{

    public class R3EConnector : IGameConnector
    {

        private MemoryMappedFile sharedMemory;
        private MemoryMappedViewAccessor sharedMemoryAccessor;
        private Thread daemonThread;
        private bool disconnect;

        private static readonly string r3EExcecutable = "RRRE";
        private static readonly string SharedMemoryName = "$Race$";

        private TimeSpan timeInterval = TimeSpan.FromMilliseconds(100);

        public event EventHandler<DataEventArgs> DataLoaded;
        public event EventHandler<EventArgs> ConnectedEvent;
        public event EventHandler<EventArgs> Disconnected;

        

        public R3EConnector()
        {
            TickTime = 10;
        }

        public bool IsConnected
        {
            get { return (sharedMemory != null && sharedMemoryAccessor != null); }
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
                sharedMemoryAccessor = sharedMemory.CreateViewAccessor(0, Marshal.SizeOf(typeof(R3ESharedData)));
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
            
            while (!disconnect && sharedMemoryAccessor.CanRead)
            {

               Thread.Sleep(TickTime);                    
               R3ESharedData data = Load();
               RaiseDataLoadedEvent(data);
            }
            sharedMemoryAccessor = null;
            sharedMemory = null;
            disconnect = false;
            RaiseDisconnectedEvent();
        }

       

        private void Disconnect()
        {
            disconnect = true;
            sharedMemoryAccessor = null;
            sharedMemory = null;
            RaiseDisconnectedEvent();
        }

        private R3ESharedData Load()
        {
            lock (sharedMemoryAccessor)
            {
                if (!IsConnected)
                    throw new InvalidOperationException("Not connected");
                R3ESharedData data;
                sharedMemoryAccessor.Read(0, out data);
                return data;
            }
        }


        private void RaiseDataLoadedEvent(R3ESharedData data)
        {
            DataEventArgs args = new DataEventArgs(FromR3EData(data));
            EventHandler<DataEventArgs> handler = DataLoaded;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        //NEED EXTRACT WHEN SUPPORT FOR OTHER SIMS IS ADDED
        private static SimulatorDataSet FromR3EData(R3ESharedData data)
        {
            SimulatorDataSet simData = new SimulatorDataSet();

            //PEDAL INFO
            simData.PedalInfo.ThrottlePedalPosition = data.ThrottlePedal;
            simData.PedalInfo.BrakePedalPosition = data.BrakePedal;

            //WaterSystemInfo
            simData.PlayerCarInfo.WaterSystmeInfo.WaterTemperature = Temperature.FromCelsius(data.EngineWaterTemp);

            //OilSystemInfo
            simData.PlayerCarInfo.OilSystemInfo.OilPressure = Pressure.FromKiloPascals(data.EngineOilPressure);
            simData.PlayerCarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(data.EngineOilTemp);

            //Brakes Info            
            simData.PlayerCarInfo.WheelsInfo.FrontLeft.BrakeTemperature = Temperature.FromCelsius(data.BrakeTemp.FrontLeft);
            simData.PlayerCarInfo.WheelsInfo.FrontRight.BrakeTemperature = Temperature.FromCelsius(data.BrakeTemp.FrontRight);
            simData.PlayerCarInfo.WheelsInfo.RearLeft.BrakeTemperature = Temperature.FromCelsius(data.BrakeTemp.RearLeft);
            simData.PlayerCarInfo.WheelsInfo.RearRight.BrakeTemperature = Temperature.FromCelsius(data.BrakeTemp.RearRight);

            //Tyre Pressure Info
            simData.PlayerCarInfo.WheelsInfo.FrontLeft.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.FrontLeft);
            simData.PlayerCarInfo.WheelsInfo.FrontRight.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.FrontRight);
            simData.PlayerCarInfo.WheelsInfo.RearLeft.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.RearLeft);
            simData.PlayerCarInfo.WheelsInfo.RearRight.TyrePressure = Pressure.FromKiloPascals(data.TirePressure.RearRight);

            //Front Left Tyre Temps
            simData.PlayerCarInfo.WheelsInfo.FrontLeft.LeftTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontLeft_Left);
            simData.PlayerCarInfo.WheelsInfo.FrontLeft.RightTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontLeft_Right);
            simData.PlayerCarInfo.WheelsInfo.FrontLeft.CenterTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontLeft_Center);

            //Front Right Tyre Temps
            simData.PlayerCarInfo.WheelsInfo.FrontRight.LeftTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontRight_Left);
            simData.PlayerCarInfo.WheelsInfo.FrontRight.RightTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontRight_Right);
            simData.PlayerCarInfo.WheelsInfo.FrontRight.CenterTyreTemp = Temperature.FromCelsius(data.TireTemp.FrontRight_Center);

            //Rear Left Tyre Temps
            simData.PlayerCarInfo.WheelsInfo.RearLeft.LeftTyreTemp = Temperature.FromCelsius(data.TireTemp.RearLeft_Left);
            simData.PlayerCarInfo.WheelsInfo.RearLeft.RightTyreTemp = Temperature.FromCelsius(data.TireTemp.RearLeft_Right);
            simData.PlayerCarInfo.WheelsInfo.RearLeft.CenterTyreTemp = Temperature.FromCelsius(data.TireTemp.RearLeft_Center);

            //Rear Right Tyre Temps
            simData.PlayerCarInfo.WheelsInfo.RearRight.LeftTyreTemp = Temperature.FromCelsius(data.TireTemp.RearRight_Left);
            simData.PlayerCarInfo.WheelsInfo.RearRight.RightTyreTemp = Temperature.FromCelsius(data.TireTemp.RearRight_Right);
            simData.PlayerCarInfo.WheelsInfo.RearRight.CenterTyreTemp = Temperature.FromCelsius(data.TireTemp.RearRight_Center);

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
