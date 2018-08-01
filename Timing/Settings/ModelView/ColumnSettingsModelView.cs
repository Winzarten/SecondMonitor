namespace SecondMonitor.Timing.Settings.ModelView
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using Properties;
    using Model;

    public class ColumnSettingsModelView : DependencyObject, INotifyPropertyChanged
    {
        public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register("Visible", typeof(bool), typeof(ColumnSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(ColumnSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });

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

        public static ColumnSettingsModelView CreateFromModel(ColumnSettings columnSettings)
        {
            ColumnSettingsModelView newModelView = new ColumnSettingsModelView();
            newModelView.FromModel(columnSettings);
            return newModelView;
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
            if (d is ColumnSettingsModelView columnSettingsModelView)
            {
                columnSettingsModelView.OnPropertyChanged(e.Property.Name);
            }
        }
    }
}