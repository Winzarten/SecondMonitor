namespace SecondMonitor.ViewModels
{
    using DataModel.Snapshot.Systems;

    public interface IWheelStatusViewModel
    {
        void ApplyWheelCondition(WheelInfo wheelInfo);
    }
}