using System;
using System.Threading;

using SecondMonitor.DataModel;
using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.DataModel.Drivers;
using SecondMonitor.PluginManager.GameConnector;

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

        private double _tyreTemp = 50;
        private double _tyreStep = 0.1;
        private double _brakeTemp = 50;
        private double _brakeStep = 1;

        private double _fuel = 2000;
        private double _totalFuel = 2000;
        private double _fuelStep = -0.01;

        private double _layoutLength = 5000;
        private double _playerLocation = 0;
        private double _playerLocationStep = 1;
        private int _playerLaps = 0;

        private double _engineWaterTemp = 30;
        private double _engineWaterTempStep = 0.01;

        private double _oilTemp = 30;
        private double _oilTempStep = 0.1;
        DateTime _lastTick = DateTime.Now;
        TimeSpan _sessionTime = new TimeSpan(0, 0, 1);

        public void ASyncConnect()
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
            Thread.Sleep(2000);
            SimulatorDataSet set = PrepareDataSet();
            RaiseSessionStartedEvent(set);
            Thread.Sleep(1000);
            while (true)
            {
                Thread.Sleep(10);
                set = PrepareDataSet();
                RaiseDataLoadedEvent(set);

                _brakeTemp += _brakeStep;
                _tyreTemp += _tyreStep;
                _fuel += _fuelStep;
                _engineWaterTemp += _engineWaterTempStep;
                _oilTemp += _oilTempStep;
                _playerLocation += _playerLocationStep;
                if (_brakeTemp > 1500 || _brakeTemp <30)
                    _brakeStep = -_brakeStep;
                if (_tyreTemp > 150 || _tyreTemp < 30)
                    _tyreStep = -_tyreStep;
                if (_fuel < 0)
                    _fuel = _totalFuel;
                if (_engineWaterTemp < 20 || _engineWaterTemp > 130)
                    _engineWaterTempStep = -_engineWaterTempStep;
                if (_oilTemp < 20 || _oilTemp > 180)
                    _oilTempStep = -_oilTempStep;
                if (_playerLocation > _layoutLength)
                {
                    _playerLocation = 0;
                    _playerLaps++;
                }

            }
        }

        private SimulatorDataSet PrepareDataSet()
        {
            DateTime tickTime = DateTime.Now;
            _sessionTime = _sessionTime.Add(tickTime.Subtract(_lastTick));                                    
            _lastTick = tickTime;
            SimulatorDataSet simulatorDataSet = new SimulatorDataSet("Test Source");            
            simulatorDataSet.SessionInfo.SessionTime = _sessionTime;
            

            
            simulatorDataSet.SessionInfo.SessionPhase = SessionInfo.SessionPhaseEnum.Green;
            simulatorDataSet.SessionInfo.LayoutLength = (float)_layoutLength;
            simulatorDataSet.SessionInfo.IsActive = true;
            simulatorDataSet.SessionInfo.SessionType = SessionInfo.SessionTypeEnum.Qualification;
            

            DriverInfo player = PrepareDriver();
            simulatorDataSet.PlayerInfo = player;
            simulatorDataSet.DriversInfo = new[] { player };
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
            driver.CompletedLaps = _playerLaps;
            driver.CurrentLapValid = true;
            driver.DistanceToPlayer = 0;
            driver.DriverName = "Lorem Ipsum";
            driver.InPits = false;
            driver.IsPlayer = true;
            driver.LapDistance = (float)_playerLocation;
            driver.CarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(_totalFuel);
            driver.CarInfo.FuelSystemInfo.FuelRemaining = Volume.FromLiters(_fuel);
            driver.CarInfo.WaterSystmeInfo.WaterTemperature = Temperature.FromCelsius(_engineWaterTemp);
            driver.CarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(_oilTemp);
            return driver;
        }

        private void UpdateWheelInfo(WheelInfo info)
        {
            info.LeftTyreTemp = Temperature.FromCelsius(_tyreTemp - 5);
            info.CenterTyreTemp = Temperature.FromCelsius(_tyreTemp);
            info.RightTyreTemp = Temperature.FromCelsius(_tyreTemp + 5);
            info.BrakeTemperature = Temperature.FromCelsius(_brakeTemp);

            info.TyrePressure = Pressure.FromKiloPascals(200);
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

        private void RaiseSessionStartedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            EventHandler<DataEventArgs> handler = SessionStarted;
            _lastTick = DateTime.Now;
            _sessionTime = new TimeSpan(0, 0, 1);
            handler?.Invoke(this, args);
        }

        private void RaiseDataLoadedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            DataLoaded?.Invoke(this, args);
        }
    }
}
