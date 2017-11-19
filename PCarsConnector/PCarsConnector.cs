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
        private readonly Queue<SimulatorDataSet> _queue = new Queue<SimulatorDataSet>();

        private DateTime lastTick;
        private TimeSpan sessionTime;

        private static readonly string r3EExcecutable = "pCARS64";
        private static readonly string SharedMemoryName = "$pcars$";

        public event EventHandler<DataEventArgs> DataLoaded;
        public event EventHandler<EventArgs> ConnectedEvent;
        public event EventHandler<EventArgs> Disconnected;
        public event EventHandler<DataEventArgs> SessionStarted;
        
        private double _nextSpeedComputation;

        internal double NextSpeedComputation
        {
            get  => _nextSpeedComputation;
            set => _nextSpeedComputation = value;
        }
       
        private uint lastSessionState = 0;
        private SimulatorDataSet previousSet = new SimulatorDataSet("PCARS");        
        private Dictionary<string, DriverInfo> previousTickInfo = new Dictionary<string, DriverInfo>();
        private readonly PCarsConvertor _pCarsConvertor;


        public PCarsConnector()
        {
            TickTime = 10;
            ResetConnector();
            _pCarsConvertor = new PCarsConvertor(this);
        }

        private void ResetConnector()
        {
            sessionTime = new TimeSpan(0);
            disconnect = false;
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

        internal TimeSpan SessionTime
        {
            get => sessionTime;
            set => sessionTime = value;
        }

        internal uint LastSessionState
        {
            get => lastSessionState;
            set => lastSessionState = value;
        }

        private void AsynConnector()
        {
            while (!TryConnect())
            {
                Thread.Sleep(10);
            }
        }

        internal Dictionary<string, DriverInfo> PreviousTickInfo
        {
            get => previousTickInfo;
            set => previousTickInfo = value;
        }

        private void StartDaemon()
        {
            if (daemonThread != null && daemonThread.IsAlive)
                throw new InvalidOperationException("Daemon is already running");
            lock (_queue)
            {
                _queue.Clear();
            }
            ResetConnector();
            daemonThread = new Thread(DaemonMethod);
            daemonThread.IsBackground = true;
            daemonThread.Start();

            Thread queueProcessorThread = new Thread(QueueProcessor) {IsBackground = true};
            queueProcessorThread.Start();
        }

        private void DaemonMethod()
        {

            while (!disconnect)
            {

                Thread.Sleep(TickTime);
                pCarsAPIStruct data = Load();
                try
                {
                    //This a state that sometimes occurs when saving pit preset during race
                    //This state is ignored, otherwise it would trigger a session reset
                    if (data.mSessionState == 0 && data.mGameState == 2)
                    {
                        continue;
                    }                    
                    DateTime tickTime = DateTime.Now;
                    TimeSpan lastTickDuration = tickTime.Subtract(lastTick);
                    SimulatorDataSet simData= _pCarsConvertor.FromPCARSData(data, lastTickDuration);
                    
                    if (lastSessionState != data.mSessionState)
                    {
                        _pCarsConvertor.Reset();
                        previousTickInfo.Clear();
                        _nextSpeedComputation = 0;
                        RaiseSessionStartedEvent(simData);
                    }

                    lastSessionState = data.mSessionState;
                    lock (_queue)
                    {
                        _queue.Enqueue(simData);
                    }                    
                    if (!IsPCarsRunning())
                        disconnect = true;
                    lastTick = tickTime;
                    previousSet = simData;
                }
                catch (PCarsConnector.NameNotFilledException)
                {
                    //Ignore, names are sometimes not set in the shared memory
                }
            }

            sharedMemory = null;
            RaiseDisconnectedEvent();
        }

        private void QueueProcessor()
        {
            while (disconnect == false)
            {
                while (_queue.Count != 0)
                {
                    SimulatorDataSet set;
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
