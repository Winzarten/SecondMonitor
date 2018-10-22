using System.Windows.Controls;
using System.Windows.Media;
using SecondMonitor.DataModel.BasicProperties;

namespace SecondMonitor.WindowsControls.WPF.Behaviors.VolumeBehavior
{
    public class BackgroundByOptimalVolumeBehavior<T> : OptimalVolumeToColorBehavior<T,Panel> where T : class, IQuantity, new()
    {
        protected override void ApplyColor(Color color)
        {
            if (AssociatedObject == null)
            {
                return;

            }

            if (AssociatedObject.Background is SolidColorBrush solidColorBrush && !solidColorBrush.IsFrozen)
            {
                solidColorBrush.Color = color;
            }
            else
            {
                AssociatedObject.Background = new SolidColorBrush(color);
            }
        }
    }
}