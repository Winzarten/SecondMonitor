namespace SecondMonitor.ViewModels.Colors
{
    using System.Windows.Media;

    public class BasicColorPaletteProvider : IColorPaletteProvider
    {
        private static readonly Color[] Colors = new[]
        {
            /*(Color) ColorConverter.ConvertFromString("#003f5c"),
            (Color) ColorConverter.ConvertFromString("#2f4b7c"),
            (Color) ColorConverter.ConvertFromString("#665191"),
            (Color) ColorConverter.ConvertFromString("#a05195"),
            (Color) ColorConverter.ConvertFromString("#d45087"),
            (Color) ColorConverter.ConvertFromString("#f95d6a"),
            (Color) ColorConverter.ConvertFromString("#ff7c43"),
            (Color) ColorConverter.ConvertFromString("#ffa600")*/
            (Color) ColorConverter.ConvertFromString("#FFE4B5"),
            (Color) ColorConverter.ConvertFromString("#ADD8E6"),
            (Color) ColorConverter.ConvertFromString("#00FF7F"),
            (Color) ColorConverter.ConvertFromString("#845EC2"),
            (Color) ColorConverter.ConvertFromString("#D65DB1"),
            (Color) ColorConverter.ConvertFromString("#FF6F91"),
            (Color) ColorConverter.ConvertFromString("#FF9671"),
            (Color) ColorConverter.ConvertFromString("#FFC75F"),
            (Color) ColorConverter.ConvertFromString("#F9F871"),
            (Color) ColorConverter.ConvertFromString("#ff7c43"),
            (Color) ColorConverter.ConvertFromString("#ffa600")
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

        public void Reset()
        {
            _currentIndex = 0;
        }
    }
}