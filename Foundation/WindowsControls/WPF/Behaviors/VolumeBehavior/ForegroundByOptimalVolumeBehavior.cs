using System.Windows.Controls;
using System.Windows.Media;
using SecondMonitor.DataModel.BasicProperties;

namespace SecondMonitor.WindowsControls.WPF.Behaviors.VolumeBehavior
{
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