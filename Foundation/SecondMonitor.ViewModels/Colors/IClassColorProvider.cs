namespace SecondMonitor.ViewModels.Colors
{
    using DataModel.BasicProperties;

    public interface IClassColorProvider
    {
        ColorDto GetColorForClass(string classId);
        void Reset();
    }
}