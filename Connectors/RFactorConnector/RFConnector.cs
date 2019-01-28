namespace SecondMonitor.RFactorConnector
{
    using System;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.InteropServices;
    using System.Threading;

    using DataModel.BasicProperties;
    using DataModel.Snapshot;

    using PluginManager.GameConnector;

    using PluginManager.DependencyChecker;

    using SharedMemory;

    public class RFConnector : AbstractGameConnector
    {
        private static readonly string[] RFExecutables = { "AMS", "rFactor", "GSC" };
        private static readonly string SharedMemoryName = "$rFactorShared$";
        private readonly TimeSpan _connectionTimeout = TimeSpan.FromSeconds(180);
        private readonly RFDataConvertor _rfDataConvertor;
        private readonly DependencyChecker dependencies;

        private DateTime _connectionTime = DateTime.MinValue;

        private MemoryMappedFile _sharedMemory;
        private string _processName;
        private int _rawLastSessionType = int.MinValue;

        private SessionPhase _lastSessionPhase;

        private SessionType _lastSessionType;

        public RFConnector()
            : base(RFExecutables)
        {
            TickTime = 16;
            dependencies = new DependencyChecker(new FileExistDependency[] { new FileExistDependency(@"Plugins\rFactorSharedMemoryMap.dll", @"Connectors\RFactor\rFactorSharedMemoryMap.dll") }, () => true);
            _rfDataConvertor = new RFDataConvertor();
        }

        public override bool IsConnected { get => _sharedMemory != null; }


        protected override void OnConnection()
        {
            ResetConnector();
            CheckDependencies();
            if (_connectionTime == DateTime.MinValue)
            {
                _connectionTime = DateTime.Now;
            }
            if (DateTime.Now - _connectionTime > _connectionTimeout)
            {
                SendMessageToClients("A rFactor based game has been detected running for extended time, but SecondMonitor wasn't able to connect to its shared memory.\n"
                                     + "Please make sure that the rFactorSharedMemoryMap.dll is correctly installed in the plugins folder.\n"
                                     + "The plugin can be downloaded at : https://github.com/dallongo/rFactorSharedMemoryMap/releases ");
                _connectionTime = DateTime.MaxValue;
            }
            _sharedMemory = MemoryMappedFile.OpenExisting(SharedMemoryName);
            _processName = Process.ProcessName;
        }

        protected override void ResetConnector()
        {
            _rawLastSessionType = int.MinValue;
            _lastSessionType = SessionType.Na;
            _lastSessionPhase = SessionPhase.Countdown;
        }

        private void CheckDependencies()
        {
            if (Process != null && !dependencies.Checked)
            {
                _connectionTime = DateTime.Now;
                string directory = Path.Combine(Path.GetPathRoot(Process.MainModule.FileName), Path.GetDirectoryName(Process.MainModule.FileName));
                Action actionToInstall = dependencies.CheckAndReturnInstallDependenciesAction(directory);
                if (actionToInstall != null)
                {
                    SendMessageToClients("A rFactor based game, "+ Process.ProcessName + ", has been detected, but the required plugin, rFactorSharedMemoryMap.dll, was not found. Do you want Second Monitor to install this plugin? You will need to restart the sim, after it is done.",
                        () => RunActionAndShowConfirmation(actionToInstall, "Operation Completed Successfully", "Unable to install the plugin, unexpected error: "));
                }
            }
        }

        private void RunActionAndShowConfirmation(Action actionToRun, string completionMessage, string errorMessage)
        {
            try
            {
                actionToRun();
                SendMessageToClients(completionMessage);
            }
            catch (Exception ex)
            {
                SendMessageToClients(errorMessage + "\n" + ex.Message);
            }
        }

        protected override string ConnectorName => "RFactor";

        protected override void DaemonMethod()
        {
            _connectionTime = DateTime.MinValue;
            while (!ShouldDisconnect)
            {
                Thread.Sleep(TickTime);
                RfShared rFactorData = Load();
                SimulatorDataSet dataSet;
                try
                {
                    dataSet = _rfDataConvertor.CreateSimulatorDataSet(rFactorData);
                    dataSet.Source = _processName;
                }
                catch (RFInvalidPackageException)
                {
                    continue;
                }

                if (CheckSessionStarted(rFactorData, dataSet))
                {
                    RaiseSessionStartedEvent(dataSet);
                }

                RaiseDataLoadedEvent(dataSet);

                if (!IsProcessRunning())
                {
                    ShouldDisconnect = true;
                }
            }
            _sharedMemory = null;
            RaiseDisconnectedEvent();
        }

        private RfShared Load()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected");
            }
            using (var view = _sharedMemory.CreateViewStream())
            {
                BinaryReader stream = new BinaryReader(view);
                byte[] buffer = stream.ReadBytes(Marshal.SizeOf(typeof(RfShared)));
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                RfShared data = (RfShared)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(RfShared));
                handle.Free();
                return data;
            }
        }

        private bool CheckSessionStarted(RfShared rfData, SimulatorDataSet dataSet)
        {
            if (_rawLastSessionType != rfData.Session || _lastSessionType != dataSet.SessionInfo.SessionType)
            {
                _lastSessionType = dataSet.SessionInfo.SessionType;
                _rawLastSessionType = rfData.Session;
                _lastSessionPhase = dataSet.SessionInfo.SessionPhase;
                return true;
            }

            if (dataSet.SessionInfo.SessionPhase != _lastSessionPhase && _lastSessionPhase != SessionPhase.Green && dataSet.SessionInfo.SessionPhase != SessionPhase.Countdown)
            {
                _lastSessionType = dataSet.SessionInfo.SessionType;
                _rawLastSessionType = rfData.Session;
                _lastSessionPhase = dataSet.SessionInfo.SessionPhase;
                return true;
            }
            return false;
        }
    }
}