using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.DataModel.Snapshot.Drivers;

namespace SecondMonitor.Remote.Common.Model
{
    using System;
    using DataModel.Snapshot;

    [Serializable]
    public class DatagramPayload
    {
        public const string Version = "SecondMonitor_RemoteVersion_1";

        public bool ContainsPlayersTiming { get; set; }

        public bool ContainsOtherDriversTiming { get; set; }

        public bool ContainsSimulatorSourceInfo { get; set; }

        public DatagramPayloadKind PayloadKind { get; set; }

        public string Source { get; set; }

        public InputInfo InputInfo { get; set; }

        public SessionInfo SessionInfo { get; set; }

        public DriverInfo[] DriversInfo { get; set; }

        public DriverInfo PlayerInfo { get; set; }

        public DriverInfo LeaderInfo { get; set; }

        public SimulatorSourceInfo SimulatorSourceInfo { get; } = new SimulatorSourceInfo();
    }
}