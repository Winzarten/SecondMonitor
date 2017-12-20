namespace SecondMonitor.Timing.GUI
{
    using System.Windows;

    using SecondMonitor.Timing.DataHandler;

    public class BindingProxy : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        #endregion

        public TimingDataViewModel Data
        {
            get => (TimingDataViewModel)GetValue(DataProperty);
            set =>  SetValue(DataProperty, value);
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(TimingDataViewModel), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
}