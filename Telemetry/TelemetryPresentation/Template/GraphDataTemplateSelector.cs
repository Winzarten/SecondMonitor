namespace SecondMonitor.TelemetryPresentation.Template
{
    using System.Windows;
    using System.Windows.Controls;

    public class GraphDataTemplateSelector : DataTemplateSelector
    {

        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return DefaultTemplate;
        }
    }
}