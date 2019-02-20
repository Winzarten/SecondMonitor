using SecondMonitor.DataModel.Snapshot;
using SecondMonitor.Remote.Common.Model;

namespace SecondMonitor.Remote.Common.Adapter
{
    public interface IDatagramPayloadAdapter
    {
        DatagramPayload CreateRegularDatagramPayload(SimulatorDataSet simulatorData);
        DatagramPayload CreateRegularSessionStartedPayload(SimulatorDataSet simulatorData);
    }
}