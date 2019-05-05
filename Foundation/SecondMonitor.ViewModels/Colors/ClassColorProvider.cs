namespace SecondMonitor.ViewModels.Colors
{
    using System.Collections.Generic;
    using DataModel.BasicProperties;

    public class ClassColorProvider : IClassColorProvider
    {
        private readonly IColorPaletteProvider _colorPaletteProvider;
        private readonly Dictionary<string, ColorDto> _cachedColors;

        public ClassColorProvider(IColorPaletteProvider colorPaletteProvider)
        {
            _colorPaletteProvider = colorPaletteProvider;
            _cachedColors = new Dictionary<string, ColorDto>();
        }

        public ColorDto GetColorForClass(string classId)
        {
            if (_cachedColors.TryGetValue(classId, out ColorDto classColor))
            {
                return classColor;
            }

            classColor = _colorPaletteProvider.GetNextAsDto();
            _cachedColors[classId] = classColor;
            return classColor;
        }

        public void Reset()
        {
            _cachedColors.Clear();
        }
    }
}