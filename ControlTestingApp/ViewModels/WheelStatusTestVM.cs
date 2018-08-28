namespace ControlTestingApp.ViewModels
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using ControlTestingApp.Annotations;

    public class WheelStatusTestVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double _tyreCondition = 50.0;

        public double TyreCondition
        {
            get => _tyreCondition;
            set
            {
                _tyreCondition = value;
                NotifyPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}