namespace SecondMonitor.ViewModels.Colors
{
    using System.Windows.Media;
    using DataModel.BasicProperties;

    public interface IColorPaletteProvider
    {
        Color GetNext();
        ColorDto GetNextAsDto();
        void Reset();
    }
}