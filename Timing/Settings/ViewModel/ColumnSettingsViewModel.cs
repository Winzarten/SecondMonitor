namespace SecondMonitor.Timing.Settings.ViewModel
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using SecondMonitor.Timing.Properties;
    using SecondMonitor.Timing.Settings.Model;

    public class ColumnSettingsViewModel : DependencyObject, INotifyPropertyChanged
    {
        public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register("Visible", typeof(bool), typeof(ColumnSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(ColumnSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Visible
        {
            get => (bool)GetValue(VisibleProperty);
            set => SetValue(VisibleProperty, value);
        }

        public double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public void FromModel(ColumnSettings columnSettings)
        {
            Visible = columnSettings.Visible;
            Width = columnSettings.Width;
        }

        public static ColumnSettingsViewModel CreateFromModel(ColumnSettings columnSettings)
        {
            ColumnSettingsViewModel newViewModel = new ColumnSettingsViewModel();
            newViewModel.FromModel(columnSettings);
            return newViewModel;
        }

        public ColumnSettings ToModel()
        {
            return new ColumnSettings { Visible = Visible, Width = Width };
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColumnSettingsViewModel columnSettingsModelView)
            {
                columnSettingsModelView.OnPropertyChanged(e.Property.Name);
            }
        }
    }
}