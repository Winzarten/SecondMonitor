namespace SecondMonitor.ViewModels
{
    using System.ComponentModel;

    public interface IAbstractViewModel<T> : IAbstractViewModel
    {
        void FromModel(T model);
        T SaveToNewModel();
    }

    public interface IAbstractViewModel : INotifyPropertyChanged
    {

    }
}