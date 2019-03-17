namespace SecondMonitor.WindowsControls.WPF.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    public class RefreshBindingOnVisibilityChangeBehavior : Behavior<ContentPresenter>
    {
        protected override void OnAttached()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            AssociatedObject.IsVisibleChanged += AssociatedObjectOnIsVisibleChanged;
        }

        private void AssociatedObjectOnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!AssociatedObject.IsVisible)
            {
                return;
            }

            AssociatedObject.GetBindingExpression(ContentPresenter.ContentProperty)?.UpdateSource();
        }
    }
}