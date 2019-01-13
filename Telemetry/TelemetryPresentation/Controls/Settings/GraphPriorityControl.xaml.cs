using System.Windows.Controls;

namespace SecondMonitor.TelemetryPresentation.Controls.Settings
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using Telemetry.TelemetryApplication.ViewModels.SettingsWindow;

    /// <summary>
    /// Interaction logic for GraphPriorityControl.xaml
    /// </summary>
    public partial class GraphPriorityControl : UserControl, INotifyPropertyChanged
    {

        public static readonly DependencyProperty LeftPanelGraphsProperty = DependencyProperty.Register(
            "LeftPanelGraphs", typeof(ObservableCollection<IGraphSettingsViewModel>), typeof(GraphPriorityControl));

        public static readonly DependencyProperty RightPanelGraphsProperty = DependencyProperty.Register(
            "RightPanelGraphs", typeof(ObservableCollection<IGraphSettingsViewModel>), typeof(GraphPriorityControl));

        public static readonly DependencyProperty NotUsedGraphsProperty = DependencyProperty.Register(
            "NotUsedGraphs", typeof(ObservableCollection<IGraphSettingsViewModel>), typeof(GraphPriorityControl));

        public ObservableCollection<IGraphSettingsViewModel> NotUsedGraphs
        {
            get => (ObservableCollection<IGraphSettingsViewModel>) GetValue(NotUsedGraphsProperty);
            set => SetValue(NotUsedGraphsProperty, value);
        }

        public ObservableCollection<IGraphSettingsViewModel> RightPanelGraphs
        {
            get => GetValue(RightPanelGraphsProperty) as ObservableCollection<IGraphSettingsViewModel>;
            set => SetValue(RightPanelGraphsProperty, value);
        }

        public ObservableCollection<IGraphSettingsViewModel> LeftPanelGraphs
        {
            get => (ObservableCollection<IGraphSettingsViewModel>) GetValue(LeftPanelGraphsProperty);
            set => SetValue(LeftPanelGraphsProperty, value);
        }

        public GraphPriorityControl()
        {
            InitializeComponent();
        }

        private void LeftPanelUpArrowClick(object sender, RoutedEventArgs e)
        {
            if (NotUsedGraphsList.SelectedItem == null)
            {
                return;
            }

            IGraphSettingsViewModel toMove = NotUsedGraphsList.SelectedItem as IGraphSettingsViewModel;
            int insertAt = LeftGraphsList.SelectedItem != null ? LeftGraphsList.SelectedIndex + 1 : LeftPanelGraphs.Count - 1;
            if (insertAt > 0 && insertAt < LeftPanelGraphs.Count)
            {
                LeftPanelGraphs.Insert(insertAt, toMove);
            }
            else
            {
                LeftPanelGraphs.Add(toMove);
            }

            NotUsedGraphs.Remove(toMove);
        }

        private void LeftPanelDownArrowClick(object sender, RoutedEventArgs e)
        {
            if (LeftGraphsList.SelectedItem == null)
            {
                return;
            }

            IGraphSettingsViewModel toMove = LeftGraphsList.SelectedItem as IGraphSettingsViewModel;
            LeftPanelGraphs.Remove(toMove);
            NotUsedGraphs.Add(toMove);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
