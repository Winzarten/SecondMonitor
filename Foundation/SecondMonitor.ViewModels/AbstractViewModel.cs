using System.Windows;

namespace SecondMonitor.ViewModels
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Properties;

    public abstract class AbstractViewModel<T> : AbstractViewModel, IAbstractViewModel<T>
    {
        public abstract void FromModel(T model);
        public abstract T SaveToNewModel();
    }

    public abstract class AbstractViewModel : DependencyObject, IAbstractViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}