namespace SecondMonitor.ViewModels.Colors
{
    using System.Windows.Media;
    using DataModel.BasicProperties;

    public class BasicColorPaletteProvider : IColorPaletteProvider
    {
        private static readonly Color[] Colors = new[]
        {
            (Color) ColorConverter.ConvertFromString("#e6194B"),
            (Color) ColorConverter.ConvertFromString("#3cb44b"),
            (Color) ColorConverter.ConvertFromString("#ffe119"),
            (Color) ColorConverter.ConvertFromString("#4363d8"),
            (Color) ColorConverter.ConvertFromString("#f58231"),
            (Color) ColorConverter.ConvertFromString("#911eb4"),
            (Color) ColorConverter.ConvertFromString("#42d4f4"),
            (Color) ColorConverter.ConvertFromString("#f032e6"),
            (Color) ColorConverter.ConvertFromString("#bfef45"),
            (Color) ColorConverter.ConvertFromString("#fabebe"),
            (Color) ColorConverter.ConvertFromString("#469990"),
            (Color) ColorConverter.ConvertFromString("#e6beff"),
            (Color) ColorConverter.ConvertFromString("#9A6324"),
            (Color) ColorConverter.ConvertFromString("#fffac8"),
            (Color) ColorConverter.ConvertFromString("#800000"),
            (Color) ColorConverter.ConvertFromString("#aaffc3"),
            (Color) ColorConverter.ConvertFromString("#808000"),
            (Color) ColorConverter.ConvertFromString("#ffd8b1"),
            (Color) ColorConverter.ConvertFromString("#000075"),

        };

        private int _currentIndex;

        public BasicColorPaletteProvider()
        {
            _currentIndex = 0;
        }

        public Color GetNext()
        {
            try
            {
                return Colors[_currentIndex];
            }
            finally
            {
                _currentIndex = (_currentIndex + 1) % Colors.Length;
            }
        }

        public ColorDto GetNextAsDto()
        {
            return ColorDto.FromColor(GetNext());
        }

        public void Reset()
        {
            _currentIndex = 0;
        }
    }
}