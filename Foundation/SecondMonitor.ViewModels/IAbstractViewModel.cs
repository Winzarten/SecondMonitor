namespace SecondMonitor.ViewModels
{
    using System.ComponentModel;

    public interface IAbstractViewModel<T> : IAbstractViewModel
    {
        T OriginalModel { get; }

        void FromModel(T model);
        T SaveToNewModel();
    }

    public interface IAbstractViewModel : INotifyPropertyChanged
    {

    }
}