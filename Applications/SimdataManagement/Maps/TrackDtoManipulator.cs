namespace SecondMonitor.SimdataManagement
{
    using System;
    using System.Linq;
    using System.Windows;
    using Contracts.TrackMap;
    using DataModel.TrackMap;

    public class TrackDtoManipulator : ITrackDtoManipulator
    {
        public TrackMapDto RotateLeft(TrackMapDto trackMapDto)
        {
            TrackGeometryDto trackGeometry = trackMapDto.TrackGeometry;
            TrackMapDto swapped = SwapAxis(trackMapDto);
            if (!trackGeometry.IsSwappedAxis)
            {
                swapped.TrackGeometry.XCoef = -trackMapDto.TrackGeometry.XCoef;
            }
            else
            {
                swapped.TrackGeometry.YCoef = -trackMapDto.TrackGeometry.YCoef;
            }

            bool reverseXAxis = Math.Sign(trackMapDto.TrackGeometry.XCoef) !=  Math.Sign(swapped.TrackGeometry.YCoef);
            bool reverseYAxis = Math.Sign(trackMapDto.TrackGeometry.YCoef) != Math.Sign(swapped.TrackGeometry.XCoef);

            SwapGeometries(swapped, reverseXAxis, reverseYAxis);
            return swapped;
        }

        public TrackMapDto RotateRight(TrackMapDto trackMapDto)
        {
            TrackGeometryDto trackGeometry = trackMapDto.TrackGeometry;
            TrackMapDto swapped = SwapAxis(trackMapDto);
            if (trackGeometry.IsSwappedAxis)
            {
                swapped.TrackGeometry.XCoef = -trackMapDto.TrackGeometry.XCoef;
            }
            else
            {
                swapped.TrackGeometry.YCoef = -trackMapDto.TrackGeometry.YCoef;
            }

            bool reverseXAxis = Math.Sign(trackMapDto.TrackGeometry.XCoef) != Math.Sign(swapped.TrackGeometry.YCoef);
            bool reverseYAxis = Math.Sign(trackMapDto.TrackGeometry.YCoef) != Math.Sign(swapped.TrackGeometry.XCoef);

            SwapGeometries(swapped, reverseXAxis, reverseYAxis);
            return swapped;

        }

        private TrackMapDto SwapAxis(TrackMapDto trackMapDto)
        {
            return new TrackMapDto()
            {
                LayoutName = trackMapDto.LayoutName,
                SimulatorSource = trackMapDto.SimulatorSource,
                TrackName = trackMapDto.TrackName,
                TrackGeometry = new TrackGeometryDto()
                {
                    ExporterVersion = trackMapDto.TrackGeometry.ExporterVersion,
                    IsSwappedAxis = !trackMapDto.TrackGeometry.IsSwappedAxis,
                    XCoef = trackMapDto.TrackGeometry.XCoef,
                    YCoef = trackMapDto.TrackGeometry.YCoef,
                    FullMapGeometry = trackMapDto.TrackGeometry.FullMapGeometry,
                    StartLineGeometry = trackMapDto.TrackGeometry.StartLineGeometry,
                    Sector1Geometry = trackMapDto.TrackGeometry.Sector1Geometry,
                    Sector2Geometry = trackMapDto.TrackGeometry.Sector2Geometry,
                    Sector3Geometry = trackMapDto.TrackGeometry.Sector3Geometry,
                }
            };
        }

        private void SwapGeometries(TrackMapDto trackMapDto, bool reverseXAxis, bool reverseYAxis)
        {
            Point[] fullMapPoints = GeometryToPoints(trackMapDto.TrackGeometry.FullMapGeometry).Select(x => new Point(reverseXAxis ? -x.Y : x.Y, reverseYAxis ? -x.X : x.X)).ToArray();
            Point[] sector1Points = GeometryToPoints(trackMapDto.TrackGeometry.Sector1Geometry).Select(x => new Point(reverseXAxis ? -x.Y : x.Y, reverseYAxis ? -x.X : x.X)).ToArray();
            Point[] sector2Points = GeometryToPoints(trackMapDto.TrackGeometry.Sector2Geometry).Select(x => new Point(reverseXAxis ? -x.Y : x.Y, reverseYAxis ? -x.X : x.X)).ToArray();
            Point[] sector3Points = GeometryToPoints(trackMapDto.TrackGeometry.Sector3Geometry).Select(x => new Point(reverseXAxis ? -x.Y : x.Y, reverseYAxis ? -x.X : x.X)).ToArray();
            Point[] startLinePoints = GeometryToPoints(trackMapDto.TrackGeometry.StartLineGeometry).Select(x => new Point(reverseXAxis ? -x.Y : x.Y, reverseYAxis ? -x.X : x.X)).ToArray();

            trackMapDto.TrackGeometry.LeftOffset = fullMapPoints.Min(x => x.X) - TrackMapFromTelemetryFactory.TrackBounds;
            trackMapDto.TrackGeometry.TopOffset = fullMapPoints.Min(x => x.Y)- TrackMapFromTelemetryFactory.TrackBounds;

            trackMapDto.TrackGeometry.Width = fullMapPoints.Max(x => x.X) + TrackMapFromTelemetryFactory.TrackBounds - trackMapDto.TrackGeometry.LeftOffset;
            trackMapDto.TrackGeometry.Height = fullMapPoints.Max(x => x.Y) + TrackMapFromTelemetryFactory.TrackBounds - trackMapDto.TrackGeometry.TopOffset;

            trackMapDto.TrackGeometry.FullMapGeometry = TrackMapFromTelemetryFactory.GetGeometry(fullMapPoints, true);
            trackMapDto.TrackGeometry.Sector1Geometry = TrackMapFromTelemetryFactory.GetGeometry(sector1Points, false);
            trackMapDto.TrackGeometry.Sector2Geometry = TrackMapFromTelemetryFactory.GetGeometry(sector2Points, false);
            trackMapDto.TrackGeometry.Sector3Geometry = TrackMapFromTelemetryFactory.GetGeometry(sector3Points, false);
            trackMapDto.TrackGeometry.StartLineGeometry = TrackMapFromTelemetryFactory.GetGeometry(startLinePoints, false);

        }

        private Point[] GeometryToPoints(string geometry)
        {
            geometry = geometry.Replace("Z", string.Empty);
            string[] splits = geometry.Split('L', 'M');
            return splits.Where(x => !string.IsNullOrWhiteSpace(x)).Select(s =>
            {
                string[] coordSplits = s.Trim().Split(' ');
                return new Point(double.Parse(coordSplits[0]), double.Parse(coordSplits[1]));
            }).ToArray();
        }
    }
}