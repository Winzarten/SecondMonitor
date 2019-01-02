using System.Windows;

namespace SecondMonitor.ViewModels
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Properties;

    public abstract class AbstractViewModel<T> : AbstractViewModel, IAbstractViewModel<T>
    {

        public T OriginalModel { get; private set; }

        protected abstract void ApplyModel(T model);

        public void FromModel(T model)
        {
            OriginalModel = model;
            ApplyModel(model);
        }
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

        protected void SetProperty<T>(ref T backingField, T value, [CallerMemberName]  string propertyName = null)
        {
            if (backingField!= null && backingField.Equals(value))
            {
                return;
            }

            backingField = value;
            NotifyPropertyChanged(propertyName);
        }
    }


}