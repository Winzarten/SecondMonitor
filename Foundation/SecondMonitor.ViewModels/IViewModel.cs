namespace SecondMonitor.ViewModels
{
    using System.ComponentModel;

    public interface IViewModel<T> : IViewModel
    {
        T OriginalModel { get; }

        void FromModel(T model);
        T SaveToNewModel();
    }

    public interface IViewModel : INotifyPropertyChanged
    {

    }
}