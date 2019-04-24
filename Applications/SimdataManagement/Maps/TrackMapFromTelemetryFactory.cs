namespace SecondMonitor.SimdataManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using DataModel.BasicProperties;
    using DataModel.Extensions;
    using DataModel.Telemetry;
    using DataModel.TrackMap;
    using Telemetry.TelemetryManagement.StoryBoard;

    public class TrackMapFromTelemetryFactory
    {
        public const int ExporterVersion = 1;
        public const int TrackBounds = 30;
        private readonly int _finishLineLength;

        public TrackMapFromTelemetryFactory() : this(TimeSpan.FromMilliseconds(10), 0)
        {

        }

        public TrackMapFromTelemetryFactory(TimeSpan mapsPointsInterval, int finishLineLength)
        {
            MapsPointsInterval = mapsPointsInterval;
            _finishLineLength = finishLineLength;
        }

        public TimeSpan MapsPointsInterval { get; set; }

        public TrackGeometryDto BuildTrackGeometryDto(TimedTelemetrySnapshots timedTelemetrySnapshots)
        {
            List<TimedTelemetrySnapshot> filteredPoints = Filter(timedTelemetrySnapshots);

            TrackGeometryDto trackGeometryDto = new TrackGeometryDto
            {
                ExporterVersion = ExporterVersion,
                FullMapGeometry = GetGeometry(filteredPoints, true),
                Sector1Geometry = GetSectorGeometry(filteredPoints, x => x.PlayerData.Timing.CurrentSector == 1),
                Sector2Geometry = GetSectorGeometry(filteredPoints, x => x.PlayerData.Timing.CurrentSector == 2),
                Sector3Geometry = GetSectorGeometry(filteredPoints, x => x.PlayerData.Timing.CurrentSector == 3),
                StartLineGeometry = ExtractFinishLine(filteredPoints),
                LeftOffset = filteredPoints.Min(x => x.PlayerData.WorldPosition.X.InMeters) - TrackBounds,
                TopOffset = filteredPoints.Min(x => x.PlayerData.WorldPosition.Z.InMeters) - TrackBounds,

            };

            trackGeometryDto.Width = filteredPoints.Max(x => x.PlayerData.WorldPosition.X.InMeters) + TrackBounds - trackGeometryDto.LeftOffset;
            trackGeometryDto.Height = filteredPoints.Max(x => x.PlayerData.WorldPosition.Z.InMeters) + TrackBounds - trackGeometryDto.TopOffset;

            return trackGeometryDto;
        }

        private string ExtractFinishLine(List<TimedTelemetrySnapshot> fullTrackPoints)
        {
            List<TimedTelemetrySnapshot> finishLinePoints = new List<TimedTelemetrySnapshot>();
            TimedTelemetrySnapshot previousPoint = null;
            IEnumerator<TimedTelemetrySnapshot> enumerator = ((IEnumerable<TimedTelemetrySnapshot>) fullTrackPoints).Reverse().GetEnumerator();
            double totalLength = 0;
            while (totalLength < _finishLineLength && enumerator.MoveNext())
            {
                TimedTelemetrySnapshot currentPoint = enumerator.Current;
                if (previousPoint != null)
                {
                    totalLength += previousPoint.PlayerData.WorldPosition.GetDistance(currentPoint.PlayerData.WorldPosition).InMeters;
                }

                finishLinePoints.Add(currentPoint);
                previousPoint = currentPoint;
            };
            enumerator.Dispose();
            return GetGeometry(finishLinePoints, false);
        }

        public static string GetGeometry(List<TimedTelemetrySnapshot> fullTrackPoints, bool wrapAround, double xCoef = 1, double yCoef = 1, bool swapXy = false)
        {
            Point[] points = ExtractWorldPoints(fullTrackPoints, xCoef, yCoef, swapXy);
            return GetGeometry(points, wrapAround);
        }

        public static string GetGeometry(IReadOnlyCollection<TimedValue> timedPoints, double xCoef = 1, double yCoef = 1, bool swapXy = false)
        {
            StringBuilder sb =new StringBuilder();
            foreach (TimedValue timedPoint in timedPoints)
            {
                Point[] points = ExtractWorldPoints(xCoef, yCoef, swapXy, timedPoint.StartSnapshot.PlayerData.WorldPosition, timedPoint.EndSnapshot.PlayerData.WorldPosition);
                sb.Append($"M {points[0].X} {points[0].Y} L  {points[1].X} {points[1].Y} ");
            }
            return sb.ToString().Replace(",", ".");
        }

        public static string GetGeometry(Point[] points, bool wrapAround)
        {
            StringBuilder sb = new StringBuilder($"M {points.First().X} {points.First().Y} ");
            points.Skip(1).ForEach(x => sb.Append($"L {x.X} {x.Y} "));
            if (wrapAround)
            {
                sb.Append($"Z");
            }

            return sb.ToString().Replace(",", ".");
        }

        private string GetSectorGeometry(List<TimedTelemetrySnapshot> fullTrackPoints, Func<TimedTelemetrySnapshot, bool> sectorPredicate)
        {
            List<TimedTelemetrySnapshot> sectorPoints = fullTrackPoints.Where(sectorPredicate).ToList();
            int nextPointIndex = (fullTrackPoints.IndexOf(sectorPoints.Last()) + 1) % fullTrackPoints.Count;
            sectorPoints.Add(fullTrackPoints[nextPointIndex]);
            return GetGeometry(sectorPoints, false);
        }

        public static Point[] ExtractWorldPoints(List<TimedTelemetrySnapshot> fullTrackPoint, double xCoef = 1, double yCoef = 1, bool swapXy = false)
        {
            return swapXy ? fullTrackPoint.Select(x => new Point(x.PlayerData.WorldPosition.Z.InMeters * yCoef, x.PlayerData.WorldPosition.X.InMeters * xCoef)).ToArray() :
            fullTrackPoint.Select(x => new Point(x.PlayerData.WorldPosition.X.InMeters * xCoef, x.PlayerData.WorldPosition.Z.InMeters * yCoef)).ToArray();
        }

        public static Point[] ExtractWorldPoints(double xCoef, double yCoef, bool swapXy, params Point3D[] points )
        {
            return swapXy ? points.Select(x => new Point(x.Z.InMeters * yCoef, x.X.InMeters * xCoef)).ToArray() :
                points.Select(x => new Point(x.X.InMeters * xCoef, x.Z.InMeters * yCoef)).ToArray();
        }

        private List<TimedTelemetrySnapshot> Filter(TimedTelemetrySnapshots timedTelemetrySnapshots)
        {
            List<TimedTelemetrySnapshot> filteredSnapshots = new List<TimedTelemetrySnapshot>();
            TimeSpan nextSnapShot = TimeSpan.Zero;;

            foreach (TimedTelemetrySnapshot snapshot in timedTelemetrySnapshots.Snapshots)
            {
                if (snapshot.LapTime < nextSnapShot)
                {
                    continue;
                }

                filteredSnapshots.Add(snapshot);
                nextSnapShot += MapsPointsInterval;
            }

            return filteredSnapshots;
        }
    }
}