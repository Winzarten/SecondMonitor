namespace SecondMonitor.WindowsControls.WPF.Behaviors.VolumeBehavior
{
    using System.Windows.Media;
    using DataModel.BasicProperties;

    public class StrokeBrushByOptimalVolumeBehavior<T> : OptimalVolumeToColorBehavior<T, ColorAbleIcon> where T : class, IQuantity, new()
    {
        protected override void ApplyColor(Color color)
        {
            if (AssociatedObject == null)
            {
                return;

            }

            if (AssociatedObject.StrokeBrush is SolidColorBrush solidColorBrush && !solidColorBrush.IsFrozen)
            {
                solidColorBrush.Color = color;
            }
            else
            {
                AssociatedObject.StrokeBrush = new SolidColorBrush(color);
            }
        }
    }
}