namespace SecondMonitor.PluginManager.Visitor
{
    using SecondMonitor.DataModel.Snapshot;

    public interface IDataSetVisitor
    {
        void Visit(SimulatorDataSet simulatorDataSet);
    }
}