namespace SecondMonitor.DataModel.BasicProperties
{
    using System;

    public class Point3D
    {
        public Point3D(Distance x, Distance y, Distance z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Distance X { get; }

        public Distance Y { get; }

        public Distance Z { get; }

        public static Distance GetDistance(Point3D p1, Point3D p2)
        {
            double distanceInM = Math.Sqrt(
                Math.Pow(p1.X.DistanceInM - p2.X.DistanceInM, 2) + Math.Pow(p1.Y.DistanceInM - p2.Y.DistanceInM, 2)
                + Math.Pow(p1.Z.DistanceInM - p2.Z.DistanceInM, 2));
            return Distance.FromMeters(distanceInM);
        }
    }
}
