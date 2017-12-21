namespace SecondMonitor.DataModel.BasicProperties
{
    public class Point3D
    {
        public Point3D( double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; }

        public double Y { get; }

        public double Z { get; }
    }
}
