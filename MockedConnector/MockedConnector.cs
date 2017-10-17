using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SecondMonitor.PluginManager.GameConnector;
using System.Threading;
using SecondMonitor.DataModel;
using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.DataModel.Drivers;

namespace SecondMonitor.MockedConnector
{
    public class MockedConnector : IGameConnector
    {
        public bool IsConnected { get; private set; }

        public int TickTime { get; set; }

        public event EventHandler<DataEventArgs> DataLoaded;
        public event EventHandler<EventArgs> ConnectedEvent;
        public event EventHandler<EventArgs> Disconnected;
        public event EventHandler<DataEventArgs> SessionStarted;

        private double tyreTemp = 50;
        private double tyreStep = 0.1;
        private double brakeTemp = 50;
        private double brakeStep = 1;

        private double fuel = 200;
        private double totalFuel = 200;
        private double fuelStep = -0.05;

        private double layoutLength = 5000;
        private double playerLocation = 0;
        private double playerLocationStep = 1;
        private int playerLaps = 0;

        private double engineWaterTemp = 30;
        private double engineWaterTempStep = 0.01;        
        DateTime lastTick = DateTime.Now;
        TimeSpan sessionTime = new TimeSpan(0, 0, 1);

        public void AsynConnect()
        {
            TryConnect();
        }

        public bool TryConnect()
        {
            IsConnected = true;
            Thread executionThread = new Thread(new ThreadStart(TestingThreadExecutor));
            executionThread.IsBackground = true;
            executionThread.Start();
            return true;
        }

        private void TestingThreadExecutor()
        {
            RaiseConnectedEvent();
            SimulatorDataSet set = PrepareDataSet();
            RaiseSessionStartedEvent(set);
            while (true)
            {
                Thread.Sleep(10);
                set = PrepareDataSet();
                RaiseDataLoadedEvent(set);

                brakeTemp += brakeStep;
                tyreTemp += tyreStep;
                fuel += fuelStep;
                engineWaterTemp += engineWaterTempStep;
                playerLocation += playerLocationStep;
                if (brakeTemp > 1500 || brakeTemp <30)
                    brakeStep = -brakeStep;
                if (tyreTemp > 150 || tyreTemp < 30)
                    tyreStep = -tyreStep;
                if (fuel < 0)
                    fuel = totalFuel;
                if (engineWaterTemp < 20 || engineWaterTemp > 130)
                    engineWaterTempStep = -engineWaterTempStep;
                if (playerLocation > layoutLength)
                {
                    playerLocation = 0;
                    playerLaps++;
                }

            }
        }

        private SimulatorDataSet PrepareDataSet()
        {
            DateTime tickTime = DateTime.Now;
            sessionTime = sessionTime.Add(tickTime.Subtract(lastTick));                                    
            lastTick = tickTime;
            SimulatorDataSet simulatorDataSet = new SimulatorDataSet();
            simulatorDataSet.SessionInfo.SessionTime = sessionTime;
            

            
            simulatorDataSet.SessionInfo.SessionPhase = SessionInfo.SessionPhaseEnum.Green;
            simulatorDataSet.SessionInfo.LayoutLength = (float)layoutLength;
            simulatorDataSet.SessionInfo.IsActive = true;
            simulatorDataSet.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.Qualification;
            

            DriverInfo player = PrepareDriver();
            simulatorDataSet.PlayerInfo = player;
            simulatorDataSet.DriversInfo = new DriverInfo[] { player };
            UpdateWheelInfo(simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.FrontLeft);
            UpdateWheelInfo(simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.FrontRight);
            UpdateWheelInfo(simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.RearRight);
            UpdateWheelInfo(simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.RearLeft);
            return simulatorDataSet;
        }

        private DriverInfo PrepareDriver()
        {
            DriverInfo driver = new DriverInfo();
            driver.CarName = "Foo car";
            driver.CompletedLaps = playerLaps;
            driver.CurrentLapValid = true;
            driver.DistanceToPlayer = 0;
            driver.DriverName = "Fedor Predkozka";
            driver.InPits = false;
            driver.IsPlayer = true;
            driver.LapDistance = (float)playerLocation;
            driver.CarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(totalFuel);
            driver.CarInfo.FuelSystemInfo.FuelRemaining = Volume.FromLiters(fuel);
            driver.CarInfo.WaterSystmeInfo.WaterTemperature = Temperature.FromCelsius(engineWaterTemp);
            return driver;
        }

        private void UpdateWheelInfo(WheelInfo info)
        {
            info.LeftTyreTemp = Temperature.FromCelsius(tyreTemp - 5);
            info.CenterTyreTemp = Temperature.FromCelsius(tyreTemp);
            info.RightTyreTemp = Temperature.FromCelsius(tyreTemp + 5);
            info.BrakeTemperature = Temperature.FromCelsius(brakeTemp);
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

        private void RaiseSessionStartedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            EventHandler<DataEventArgs> handler = SessionStarted;
            lastTick = DateTime.Now;
            sessionTime = new TimeSpan(0, 0, 1);
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
    }
}
