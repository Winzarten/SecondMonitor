using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace SecondMonitor.DataModel.BasicProperties
{
    [Serializable]
    public class ColorDTO
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

        public static ColorDTO FromColor(Color color)
        {
            return new ColorDTO()
            {
                Alpha = color.A,
                Red = color.R,
                Green = color.G,
                Blue = color.B
            };
        }
    }
}