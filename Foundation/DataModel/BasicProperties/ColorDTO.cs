using System;
using System.Xml.Serialization;

namespace SecondMonitor.DataModel.BasicProperties
{
    using System.Windows.Media;

    [Serializable]
    public sealed class ColorDto
    {
        [XmlAttribute]
        public byte Alpha { get; set; }

        [XmlAttribute]
        public byte Red { get; set; }

        [XmlAttribute]
        public byte Green { get; set; }

        [XmlAttribute]
        public byte Blue { get; set; }

        public Color ToColor()
        {
            return Color.FromArgb(Alpha, Red, Green, Blue);
        }

        public SolidColorBrush ToSolidColorBrush()
        {
            return new SolidColorBrush(ToColor());
        }

        public static ColorDto FromColor(Color color)
        {
            return new ColorDto()
            {
                Alpha = color.A,
                Blue = color.B,
                Green = color.G,
                Red = color.R,
            };
        }
    }
}