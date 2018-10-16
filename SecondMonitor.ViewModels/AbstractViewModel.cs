namespace SecondMonitor.ViewModels
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Annotations;

    public abstract class AbstractViewModel<T> : AbstractViewModel
    {
        public abstract void FromModel(T model);
        public abstract T SaveToNewModel();
    }

    public abstract class AbstractViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}