namespace SecondMonitor.ViewModels
{
    using WindowsControls.Properties;
    using DataModel.Snapshot;

    public interface ISimulatorDataSetViewModel
    {
        void ApplyDateSet([NotNull]SimulatorDataSet dataSet);

        void Reset();
    }
}