namespace SecondMonitor.ViewModels
{
    using DataModel.Snapshot;
    using Properties;

    public interface ISimulatorDataSetViewModel
    {
        void ApplyDateSet([NotNull]SimulatorDataSet dataSet);

        void Reset();
    }
}