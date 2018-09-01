namespace SecondMonitor.MockedConnector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;
    using SecondMonitor.DataModel.Snapshot.Systems;
    using SecondMonitor.PluginManager.GameConnector;

    public class MockedConnector : IGameConnector
    {
        public event EventHandler<MessageArgs> DisplayMessage;

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

        private double _layoutLength = 2000;

        private readonly Dictionary<string, DriverInfo> _players = new Dictionary<string, DriverInfo>();

        private double _playerLocationStep = 1;

        private double _engineWaterTemp = 30;
        private double _engineWaterTempStep = 0.01;

        private double _oilTemp = 30;
        private double _oilTempStep = 0.1;

        private DateTime _lastTick = DateTime.Now;
        private TimeSpan _sessionTime = new TimeSpan(0, 0, 1);

        public void ASyncConnect()
        {
            TryConnect();
        }

        public bool TryConnect()
        {

            #if !DEBUG
            IsConnected = true;
            Thread executionThread = new Thread(new ThreadStart(TestingThreadExecutor));
            executionThread.IsBackground = true;
            executionThread.Start();
            return true;
    #else
            return false;
            #endif


        }

        private void TestingThreadExecutor()
        {
            RaiseConnectedEvent();
            Thread.Sleep(2000);
            this.ConnectDriver("Lorem Ipsum", true);
            SimulatorDataSet set = PrepareDataSet();
            RaiseSessionStartedEvent(set);
            Thread.Sleep(1000);
            while (true)
            {
                if (set.SessionInfo.SessionTime.TotalSeconds > 10 && !this._players.ContainsKey("Driver 2") && set.SessionInfo.SessionTime.TotalSeconds < 60)
                {
                    this.ConnectDriver("Driver 2", false);
                }

                if (set.SessionInfo.SessionTime.TotalSeconds > 70 && this._players.ContainsKey("Driver 2") && set.SessionInfo.SessionTime.TotalSeconds < 80 )
                {
                    this._players.Remove("Driver 2");
                }

                if (set.SessionInfo.SessionTime.TotalSeconds > 120 && !this._players.ContainsKey("Driver 2"))
                {
                    this.ConnectDriver("Driver 2", false);
                }

                if (set.SessionInfo.SessionTime.TotalSeconds > 12)
                {
                    this.ConnectDriver("Driver 3", false);
                }

                if (set.SessionInfo.SessionTime.TotalSeconds > 15)
                {
                    this.ConnectDriver("Driver 4", false);
                }

                if (set.SessionInfo.SessionTime.TotalSeconds > 35)
                {
                    this.ConnectDriver("Driver 5", false);
                }
                Thread.Sleep(10);
                set = PrepareDataSet();
                RaiseDataLoadedEvent(set);

                _brakeTemp += _brakeStep;
                _tyreTemp += _tyreStep;
                _fuel += _fuelStep;
                _engineWaterTemp += _engineWaterTempStep;
                _oilTemp += _oilTempStep;

                if (_brakeTemp > 1500 || _brakeTemp < 30)
                {
                    _brakeStep = -_brakeStep;
                }

                if (_tyreTemp > 150 || _tyreTemp < 30)
                {
                    _tyreStep = -_tyreStep;
                }

                if (_fuel < 0)
                {
                    _fuel = _totalFuel;
                }

                if (_engineWaterTemp < 20 || _engineWaterTemp > 130)
                {
                    _engineWaterTempStep = -_engineWaterTempStep;
                }

                if (_oilTemp < 20 || _oilTemp > 180)
                {
                    _oilTempStep = -_oilTempStep;
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

            simulatorDataSet.SessionInfo.SessionPhase = SessionPhase.Green;
            simulatorDataSet.SessionInfo.TrackInfo.LayoutLength = (float)_layoutLength;
            simulatorDataSet.SessionInfo.IsActive = true;
            simulatorDataSet.SessionInfo.SessionType = SessionType.Qualification;

            foreach (DriverInfo driver in this._players.Values)
            {
                UpdateDriver(driver);
                if (driver.IsPlayer)
                {
                    simulatorDataSet.PlayerInfo = driver;
                }
            }
            simulatorDataSet.DriversInfo = this._players.Values.ToArray();
            simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.FrontLeft = new WheelInfo();
            simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.FrontRight = new WheelInfo();
            simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.RearRight = new WheelInfo();
            simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.RearLeft = new WheelInfo();
            UpdateWheelInfo(simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.FrontLeft);
            UpdateWheelInfo(simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.FrontRight);
            UpdateWheelInfo(simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.RearRight);
            UpdateWheelInfo(simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.RearLeft);
            return simulatorDataSet;
        }

        private void ConnectDriver(string name, bool isPlayer)
        {
            if (this._players.ContainsKey(name))
            {
                return;
            }
            DriverInfo driver = new DriverInfo();
            this._players[name] = driver;
            driver.CarName = "Foo car";
            driver.CompletedLaps = 0;
            driver.CurrentLapValid = true;
            driver.DistanceToPlayer = 0;
            driver.DriverName = name;
            driver.InPits = false;
            driver.IsPlayer = isPlayer;
            driver.LapDistance = 0;
            driver.Position = this._players.Values.Count;
            if (!isPlayer)
            {
                return;
            }

            driver.CarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(_totalFuel);
            driver.CarInfo.FuelSystemInfo.FuelRemaining = Volume.FromLiters(_fuel);
            driver.CarInfo.WaterSystemInfo.WaterTemperature = Temperature.FromCelsius(_engineWaterTemp);
            driver.CarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(_oilTemp);
            return;
        }

        private void UpdateDriver(DriverInfo driverInfo)
        {
            driverInfo.LapDistance += this._playerLocationStep;
            driverInfo.TotalDistance += _playerLocationStep;

            if (driverInfo.LapDistance >= this._layoutLength)
            {
                driverInfo.LapDistance = 0;
                driverInfo.CompletedLaps++;
            }

            if (!driverInfo.IsPlayer)
            {
                return;
            }

            driverInfo.CarInfo.FuelSystemInfo.FuelCapacity = Volume.FromLiters(_totalFuel);
            driverInfo.CarInfo.FuelSystemInfo.FuelRemaining = Volume.FromLiters(_fuel);
            driverInfo.CarInfo.WaterSystemInfo.WaterTemperature = Temperature.FromCelsius(_engineWaterTemp);
            driverInfo.CarInfo.OilSystemInfo.OilTemperature = Temperature.FromCelsius(_oilTemp);
        }

        private void UpdateWheelInfo(WheelInfo info)
        {
            info.LeftTyreTemp.ActualQuantity = Temperature.FromCelsius(_tyreTemp - 5);
            info.CenterTyreTemp.ActualQuantity = Temperature.FromCelsius(_tyreTemp);
            info.RightTyreTemp.ActualQuantity = Temperature.FromCelsius(_tyreTemp + 5);
            info.TyreCoreTemperature.ActualQuantity = Temperature.FromCelsius(_tyreTemp);
            info.BrakeTemperature.ActualQuantity = Temperature.FromCelsius(_brakeTemp);

            info.TyrePressure.ActualQuantity = Pressure.FromKiloPascals(200);
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
