using System.Windows.Media;

namespace SecondMonitor.WindowsControls.WPF.Behaviors.TyreWear
{
    public class ColorAbleIconByLimits : ColorByLinearLimitsBehavior<ColorAbleIcon>
    {
        protected override void ApplyColor(Color color)
        {
            if (AssociatedObject != null)
            {
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
}