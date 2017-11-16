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
using SecondMonitor.DataModel.Drivers;

namespace SecondMonitor.R3EConnector
{

    public class R3EConnector : IGameConnector
    {

        private MemoryMappedFile sharedMemory;
        private readonly Queue<SimulatorDataSet> _queue = new Queue<SimulatorDataSet>();
        private Thread daemonThread;
        private bool disconnect;
        private bool inSession;
        private int lastSessionType;
        private int lastSessionPhase;
        private Dictionary<string, DriverInfo> lastTickInformation;

        private DateTime lastTick;
        private TimeSpan sessionTime;
        private Double sessionStartR3RTime;



        private static readonly string r3EExcecutable = "RRRE";
        private static readonly string SharedMemoryName = "$R3E";

        private TimeSpan timeInterval = TimeSpan.FromMilliseconds(100);

        public event EventHandler<DataEventArgs> DataLoaded;
        public event EventHandler<DataEventArgs> SessionStarted;
        public event EventHandler<EventArgs> ConnectedEvent;
        public event EventHandler<EventArgs> Disconnected;

        private R3EDataConvertor DataConvertor { get; set; }



        public R3EConnector()
        {
            DataConvertor = new R3EDataConvertor(this);
            TickTime = 10;
            ResetConnector();
        }

        private void ResetConnector()
        {
            inSession = false;
            lastTick = DateTime.Now;
            lastTickInformation = new Dictionary<string, DriverInfo>();
            lastSessionType = -2;
            sessionTime = new TimeSpan(0);
            sessionStartR3RTime = 0;
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

        internal TimeSpan SessionTime { get => sessionTime; }

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
            if (daemonThread != null && daemonThread.IsAlive)
                throw new InvalidOperationException("Daemon is already running");
            ResetConnector();
            disconnect = false;
            _queue.Clear();
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
                SimulatorDataSet data = DataConvertor.FromR3EData(r3rData);
                if (CheckSessionStarted(r3rData))
                {
                    lastTickInformation.Clear();
                    RaiseSessionStartedEvent(data);
                }
                DateTime tickTime = DateTime.Now;
                if (r3rData.GamePaused != 1 && r3rData.ControlType != (int) Constant.Control.Replay)
                {
                    //sessionTime = sessionTime.Add(tickTime.Subtract(lastTick));
                    sessionTime = TimeSpan.FromSeconds(r3rData.Player.GameSimulationTime - sessionStartR3RTime);
                }
                lastTick = tickTime;
                lock(_queue)
                {
                    _queue.Enqueue(data);
                }
                if (r3rData.ControlType == -1 && !IsRrreRunning())
                    disconnect = true;
            }
            sharedMemory = null;
            RaiseDisconnectedEvent();
        }

        private void QueueProcessor()
        {
            while (disconnect == false)
            {
                SimulatorDataSet set;
                while (_queue.Count != 0)
                {
                    lock (_queue)
                    {
                        set = _queue.Dequeue();
                    }
                    RaiseDataLoadedEvent(set);
                }
                Thread.Sleep(TickTime);
            }
            _queue.Clear();
        }

        private DriverInfo GetLastTickInfo(string driverName)
        {
            if (lastTickInformation.ContainsKey(driverName))
                return lastTickInformation[driverName];
            return null;
        }

        internal void StoreLastTickInfo(DriverInfo driverInfo)
        {
            lastTickInformation[driverInfo.DriverName] = driverInfo;
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
            if (r3rData.SessionType != lastSessionType)
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
            handler?.Invoke(this, args);
        }

        private void RaiseDataLoadedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            DataLoaded?.Invoke(this, args);
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