namespace SecondMonitor.DataModel.Snapshot
{
    public interface ISimulatorDateSetVisitor
    {
        void Visit(SimulatorDataSet simulatorDataSet);
        void Reset();
    }
}