using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace SecondMonitor.DataModel.BasicProperties
{
    [Serializable]
    public sealed class ColorDto
    {
        [XmlAttribute]
        public byte Alpha;
        [XmlAttribute]
        public byte Red;
        [XmlAttribute]
        public byte Green;
        [XmlAttribute]
        public byte Blue;

        public Color ToColor()
        {
            return Color.FromArgb(Alpha, Red, Green, Blue);
        }

        public static ColorDto FromColor(Color color)
        {
            return new ColorDto()
            {
                Alpha = color.A,
                Red = color.R,
                Green = color.G,
                Blue = color.B
            };
        }
    }
}