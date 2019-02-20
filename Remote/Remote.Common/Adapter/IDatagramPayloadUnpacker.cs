namespace SecondMonitor.Remote.Common.Adapter
{
    using DataModel.Snapshot;
    using Model;

    public interface IDatagramPayloadUnPacker
    {
        SimulatorDataSet UnpackDatagramPayload(DatagramPayload datagramPayload);
    }
}