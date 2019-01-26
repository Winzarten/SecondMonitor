namespace SecondMonitor.TelemetryPresentation.Template
{
    using System.Windows;
    using System.Windows.Controls;
    using Telemetry.TelemetryApplication.ViewModels.GraphPanel.Chassis;
    using Telemetry.TelemetryApplication.ViewModels.GraphPanel.Wheels;

    public class GraphDataTemplateSelector : DataTemplateSelector
    {

        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate WheelsGraphTemplate { get; set; }
        public DataTemplate TyreTemperaturesGraphTemplate { get; set; }
        public DataTemplate ChassisGraphTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (item)
            {
                case AbstractWheelsGraphViewModel _:
                    return WheelsGraphTemplate;
                case AbstractTyreTemperaturesViewModel _:
                    return TyreTemperaturesGraphTemplate;
                case AbstractChassisGraphViewModel _:
                    return ChassisGraphTemplate;
                default:
                    return DefaultTemplate;
            }
        }
    }
}