namespace SecondMonitor.PluginManager.Visitor
{
    using DataModel.Snapshot;

    public interface IDataSetVisitor
    {
        void Visit(SimulatorDataSet simulatorDataSet);
    }
}