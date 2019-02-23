namespace SecondMonitor.Remote.Common.Comparators
{
    using DataModel.Snapshot;

    public interface ISimulatorSourceInfoComparator
    {
        bool AreEqual(SimulatorSourceInfo x, SimulatorSourceInfo y);
    }
}