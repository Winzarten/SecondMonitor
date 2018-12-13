namespace SecondMonitor.DataModel.BasicProperties
{
    using System;

    [Serializable]
    public class Point3D
    {
        public Point3D()
        {

        }

        public Point3D(Distance x, Distance y, Distance z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Distance X { get; }

        public Distance Y { get; }

        public Distance Z { get; }

        public Distance GetDistance(Point3D p2)
        {
            return GetDistance(this, p2);
        }

        public static Distance GetDistance(Point3D p1, Point3D p2)
        {
            double distanceInM = Math.Sqrt(
                Math.Pow(p1.X.InMeters - p2.X.InMeters, 2) + Math.Pow(p1.Y.InMeters - p2.Y.InMeters, 2)
                + Math.Pow(p1.Z.InMeters - p2.Z.InMeters, 2));
            return Distance.FromMeters(distanceInM);
        }
    }
}
