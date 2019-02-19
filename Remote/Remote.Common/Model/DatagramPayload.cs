namespace SecondMonitor.Remote.Common.Model
{
    using System;
    using DataModel.Snapshot;

    [Serializable]
    public class DatagramPayload
    {
        public const string Version = "SecondMonitor_RemoteVersion_1";

        public DatagramPayloadKind PayloadKind { get; set; }

        public SimulatorDataSet Payload { get; set; }
    }
}