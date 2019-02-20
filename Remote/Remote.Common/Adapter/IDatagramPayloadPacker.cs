using SecondMonitor.DataModel.Snapshot;
using SecondMonitor.Remote.Common.Model;

namespace SecondMonitor.Remote.Common.Adapter
{
    public interface IDatagramPayloadPacker
    {
        bool IsMinimalPackageDelayPassed();
        DatagramPayload CreateHandshakeDatagramPayload();
        DatagramPayload CreateHearthBeatDatagramPayload();
        DatagramPayload CreateRegularDatagramPayload(SimulatorDataSet simulatorData);
        DatagramPayload CreateSessionStartedPayload(SimulatorDataSet simulatorData);
    }
}