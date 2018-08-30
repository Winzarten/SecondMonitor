namespace SecondMonitor.WindowsControls.Colors
{
    using System.Windows.Media;

    public static class MediaColorExtension
    {
        public static System.Drawing.Color ToDrawingColor(this Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Color ToMediaColor(this System.Drawing.Color drawingColor)
        {
            return new Color() { A = drawingColor.A, B = drawingColor.B, G = drawingColor.G, R = drawingColor.R };
        }

        public static Color InterpolateHslColor(Color color1, Color color2, double lambda)
        {
            HSLColor hslColor1 = HSLColor.FromColor(color1.ToDrawingColor());
            HSLColor hslColor2 = HSLColor.FromColor(color2.ToDrawingColor());
            return HSLColor.Interpolate(hslColor1, hslColor2, lambda).ToColor().ToMediaColor();
        }

        public static Color InterpolateRGB(Color a, Color b, double t)
        {
            Color computedColor = new Color()
                                      {
                                          R = (byte)(a.R + (b.R - a.R) * t),
                                          G = (byte)(a.G + (b.G - a.G) * t),
                                          B = (byte)(a.B + (b.B - a.B) * t),
                                          A = (byte)(a.A + (b.A - a.A) * t)
                                      };
            return computedColor;

        }
    }
}
