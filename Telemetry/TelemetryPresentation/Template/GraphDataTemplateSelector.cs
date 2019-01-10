namespace SecondMonitor.TelemetryPresentation.Template
{
    using System.Windows;
    using System.Windows.Controls;
    using Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels;

    public class GraphDataTemplateSelector : DataTemplateSelector
    {

        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate WheelsGraphTemplate { get; set; }
        public DataTemplate TyreTemperaturesGraphTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is AbstractWheelsGraphViewModel)
            {
                return WheelsGraphTemplate;
            }

            if (item is AbstractTyreTemperaturesViewModel)
            {
                return TyreTemperaturesGraphTemplate;
            }

            return DefaultTemplate;
        }
    }
}