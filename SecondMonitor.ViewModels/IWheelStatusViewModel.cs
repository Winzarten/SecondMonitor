namespace SecondMonitor.ViewModels
{
    using SecondMonitor.DataModel.Snapshot.Systems;

    public interface IWheelStatusViewModel
    {
        void ApplyWheelCondition(WheelInfo wheelInfo);
    }
}