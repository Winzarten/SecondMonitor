namespace SecondMonitor.ViewModels
{
    using DataModel.Snapshot;

    public interface ISimulatorDataSetViewModel
    {
        void ApplyDateSet(SimulatorDataSet dataSet);

        void Reset();
    }
}