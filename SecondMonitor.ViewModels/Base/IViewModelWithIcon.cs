namespace SecondMonitor.ViewModels.Base
{
    using System.Windows.Media;

    public interface IViewModelWithIcon
    {
        ImageSource Icon
        {
            get;
        }
    }
}