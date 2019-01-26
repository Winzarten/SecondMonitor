namespace SecondMonitor.ViewModels.Colors
{
    using System.Windows.Media;

    public interface IColorPaletteProvider
    {
        Color GetNext();
        void Reset();
    }
}