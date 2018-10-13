namespace SecondMonitor.WindowsControls.WPF.Behaviors
{
    using System.Windows.Controls;
    using System.Windows.Media;

    using DataModel.BasicProperties;

    public class ForegroundByOptimalVolumeBehavior<T> : OptimalVolumeToColorBehavior<T, Control> where T : class, IQuantity, new()
    {
        protected override void ApplyColor(Color color)
        {
            if (AssociatedObject != null)
            {
                if (AssociatedObject.Foreground is SolidColorBrush solidColorBrush && !solidColorBrush.IsFrozen)
                {
                    solidColorBrush.Color = color;
                }
                else
                {
                    AssociatedObject.Foreground = new SolidColorBrush(color);
                }
            }
        }
    }
}