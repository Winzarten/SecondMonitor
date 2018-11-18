namespace SecondMonitor.MockedConnector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;
    using DataModel.Snapshot.Systems;
    using PluginManager.GameConnector;

    public class MockedConnector : IGameConnector
    {
#pragma warning disable CS0067
        public event EventHandler<MessageArgs> DisplayMessage;
#pragma warning restore CS0067

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

        private double _layoutLength = 5200;

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

            #if DEBUG
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
            ConnectDriver("Lorem Ipsum", true);
            SimulatorDataSet set = PrepareDataSet();
            RaiseSessionStartedEvent(set);
            Thread.Sleep(1000);
            while (true)
            {
                if (set.SessionInfo.SessionTime.TotalSeconds > 10 && !_players.ContainsKey("Driver 2") && set.SessionInfo.SessionTime.TotalSeconds < 60)
                {
                    ConnectDriver("Driver 2", false);
                }

                if (set.SessionInfo.SessionTime.TotalSeconds > 70 && _players.ContainsKey("Driver 2") && set.SessionInfo.SessionTime.TotalSeconds < 80 )
                {
                    _players.Remove("Driver 2");
                }

                if (set.SessionInfo.SessionTime.TotalSeconds > 120 && !_players.ContainsKey("Driver 2"))
                {
                    ConnectDriver("Driver 2", false);
                }

                if (set.SessionInfo.SessionTime.TotalSeconds > 12)
                {
                    ConnectDriver("Driver 3", false);
                }

                if (set.SessionInfo.SessionTime.TotalSeconds > 15)
                {
                    ConnectDriver("Driver 4", false);
                }

                if (set.SessionInfo.SessionTime.TotalSeconds > 35)
                {
                    ConnectDriver("Driver 5", false);
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
            simulatorDataSet.SimulatorSourceInfo.WorldPositionInvalid = true;
            simulatorDataSet.SessionInfo.TrackInfo.TrackName = "Slovakia Ring";
            simulatorDataSet.SessionInfo.TrackInfo.TrackLayoutName = "Grand Prix";
            simulatorDataSet.SessionInfo.TrackInfo.LayoutLength = Distance.FromMeters(_layoutLength);
            simulatorDataSet.SessionInfo.WeatherInfo.AirTemperature = Temperature.FromCelsius(25);
            simulatorDataSet.SessionInfo.WeatherInfo.TrackTemperature = Temperature.FromCelsius(31);
            simulatorDataSet.SessionInfo.WeatherInfo.RainIntensity = 0;
            simulatorDataSet.SessionInfo.IsActive = true;
            simulatorDataSet.SimulatorSourceInfo.GlobalTyreCompounds = true;
            simulatorDataSet.SessionInfo.SessionType = SessionType.Qualification;

            foreach (DriverInfo driver in _players.Values)
            {
                UpdateDriver(driver);
                if (driver.IsPlayer)
                {
                    simulatorDataSet.PlayerInfo = driver;
                }
            }
            simulatorDataSet.DriversInfo = _players.Values.ToArray();
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
            if (_players.ContainsKey(name))
            {
                return;
            }
            DriverInfo driver = new DriverInfo();
            _players[name] = driver;
            driver.CarName = "Mazda 626";
            driver.CompletedLaps = 0;
            driver.CurrentLapValid = true;
            driver.DistanceToPlayer = 0;
            driver.DriverName = name;
            driver.InPits = false;
            driver.IsPlayer = isPlayer;
            driver.LapDistance = 0;
            driver.Position = _players.Values.Count;
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
            driverInfo.LapDistance += _playerLocationStep;
            driverInfo.TotalDistance += _playerLocationStep;

            if (driverInfo.LapDistance >= _layoutLength)
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
            info.TyreType = "Slick";
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
