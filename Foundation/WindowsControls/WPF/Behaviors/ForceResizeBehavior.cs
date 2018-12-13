namespace SecondMonitor.WindowsControls.WPF.Behaviors
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows.Interactivity;

    using LiveCharts.Wpf;

    public class ForceResizeBehavior : Behavior<AngularGauge>
    {

        protected override void OnAttached()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            DependencyPropertyDescriptor pd = DependencyPropertyDescriptor.FromProperty(AngularGauge.FromValueProperty, typeof(AngularGauge));
            pd.AddValueChanged(AssociatedObject, Handler);
            pd = DependencyPropertyDescriptor.FromProperty(AngularGauge.ToValueProperty, typeof(AngularGauge));
            pd.AddValueChanged(AssociatedObject, Handler);
        }

        private void Handler(object sender, EventArgs e)
        {
            //typeof(AngularGauge).GetMethod("Draw", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(AssociatedObject, null);
        }
    }
}