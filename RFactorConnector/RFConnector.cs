namespace SecondMonitor.RFactorConnector
{
    using System;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.InteropServices;
    using System.Threading;

    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.PluginManager.GameConnector;
    using SecondMonitor.RFactorConnector.SharedMemory;

    public class RFConnector : AbstractGameConnector
    {
        private static readonly string[] RFExecutables = { "AMS" };
        private static readonly string SharedMemoryName = "$rFactorShared$";
        private MemoryMappedFile _sharedMemory;

        public RFConnector()
            : base(RFExecutables)
        {
            TickTime = 10;
        }

        public override bool IsConnected { get => _sharedMemory != null; }


        protected override void OnConnection()
        {
            this.ResetConnector();
            _sharedMemory = MemoryMappedFile.OpenExisting(SharedMemoryName);
        }

        protected override void ResetConnector()
        {

        }

        protected override string ConnectorName => "RFactor";

        protected override void DaemonMethod()
        {
            while (!this.ShouldDisconnect)
            {
                Thread.Sleep(TickTime);
                RfShared rFactorData = Load();

                RaiseDataLoadedEvent(new SimulatorDataSet(ConnectorName));

                if (!this.IsProcessRunning())
                {
                    ShouldDisconnect = true;
                }
            }
            this._sharedMemory = null;
            this.RaiseDisconnectedEvent();
        }

        private RfShared Load()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected");
            }

            MemoryMappedViewStream view = _sharedMemory.CreateViewStream();
            BinaryReader stream = new BinaryReader(view);
            byte[] buffer = stream.ReadBytes(Marshal.SizeOf(typeof(RfShared)));
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            RfShared data = (RfShared)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(RfShared));
            handle.Free();
            return data;
        }
    }
}